using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaFacturacion.Models;

namespace SistemaFacturacion.Controllers
{
    /// <summary>
    /// Controller to manage CRUD operations for products.
    /// </summary>
    /// <remarks>
    /// Protected so that only users with the "Administrador" role can access it.
    /// </remarks>
    [Authorize(Roles = "Administrador")] // "Administrador" is left as is, as it's a role name string.
    public class ProductsController : Controller
    {
        private readonly BillingSystemDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductsController"/> class.
        /// </summary>
        /// <param name="context">The database context, injected via dependency injection.</param>
        public ProductsController(BillingSystemDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Displays a list of products, allowing searching by code, name, or category.
        /// </summary>
        /// <param name="searchTerm">The text to search for within the products.</param>
        /// <returns>A view with the filtered list of products.</returns>
        public async Task<IActionResult> Index(string searchTerm)
        {
            ViewData["FiltroActual"] = searchTerm;

            var products = from p in _context.Products
                            select p;

            if (!String.IsNullOrEmpty(searchTerm))
            {
                // Logica para hacer que se muestre el product que se busque por Code, Name o Category, insensible a mayúsculas/minúsculas.
                var upperCaseSearchTerm = searchTerm.ToUpper();

                products = products.Where(p =>
                    p.Code.ToUpper().Contains(upperCaseSearchTerm) ||
                    p.Name.ToUpper().Contains(upperCaseSearchTerm) ||
                    p.Category.ToUpper().Contains(upperCaseSearchTerm)
                );
            }

            return View(await products.ToListAsync());
        }

        /// <summary>
        /// Displays the details of a specific product.
        /// </summary>
        /// <param name="id">The ID of the product to display.</param>
        /// <returns>The details view for the product.</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        /// <summary>
        /// Displays the form to create a new product.
        /// </summary>
        /// <returns>The create view.</returns>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Processes the request to create a new product.
        /// The product code is automatically generated after the first save.
        /// </summary>
        /// <param name="product">The Product object with data from the form.</param>
        /// <returns>A redirection to the Index action if successful, or the same view with errors.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Price,Category,IsActive")] Product product)
        {
            // Remove validation for Code and Stock, as they will be assigned manually.
            ModelState.Remove("Code");
            ModelState.Remove("Stock");

            if (ModelState.IsValid)
            {
                // 1. Assign initial values before the first save.
                product.Stock = 0;
                product.Code = "TEMP"; // Temporary value to pass non-nullable DB constraints.

                // 2. Save the product to the DB. After this line, EF populates the product.Id property.
                _context.Add(product);
                await _context.SaveChangesAsync();

                // 3. Now that we have the Id, create the unique code and update the record.
                // "D5" formats the number to 5 digits, padding with zeros (e.g., 00042).
                product.Code = "PROD-" + product.Id.ToString("D5");
                _context.Update(product);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Product '{product.Name}' creado exitosamente con el código {product.Code}.";
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        /// <summary>
        /// Displays the form to edit an existing product.
        /// </summary>
        /// <param name="id">The ID of the product to edit.</param>
        /// <returns>The edit view with the product's data.</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        /// <summary>
        /// Processes the request to update a product.
        /// Ensures that the 'Code' and 'Stock' fields cannot be modified from the form.
        /// </summary>
        /// <param name="id">The ID of the product being edited.</param>
        /// <param name="productViewModel">The product data submitted from the form.</param>
        /// <returns>A redirection to the Index action if successful, or the edit view with errors.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,Category,IsActive")] Product productViewModel)
        {
            if (id != productViewModel.Id)
            {
                return NotFound();
            }

            // Remove validation for fields that should not be edited by the user.
            ModelState.Remove("Code");
            ModelState.Remove("Stock");

            if (ModelState.IsValid)
            {
                try
                {
                    // Get the original product from the database without tracking it.
                    var productoOriginal = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
                    if (productoOriginal == null)
                    {
                        return NotFound();
                    }

                    // Transfer the values that should not change.
                    productViewModel.Code = productoOriginal.Code;
                    productViewModel.Stock = productoOriginal.Stock;

                    /// Update the complete entity.
                    _context.Update(productViewModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductoExists(productViewModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(productViewModel);
        }

        /// <summary>
        /// Displays the confirmation page for deleting (deactivating) a product.
        /// </summary>
        /// <param name="id">The ID of the product.</param>
        /// <returns>The delete confirmation view.</returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        /// <summary>
        /// Confirms the logical deletion (soft delete) of a product.
        /// Instead of deleting the record, it sets its 'IsActive' property to false.
        /// </summary>
        /// <param name="id">The ID of the product to deactivate.</param>
        /// <returns>A redirection to the Index action.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var producto = await _context.Products.FindAsync(id);
            if (producto != null)
            {
                // Soft Delete: Instead of removing, we change the state and update.
                producto.IsActive = false;
                _context.Update(producto);
                await _context.SaveChangesAsync();
                TempData["InfoMessage"] = $"El product '{producto.Name}' ha sido desactivado.";
            }

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Private helper method to check if a product exists in the database.
        /// </summary>
        /// <param name="id">The ID of the product to check.</param>
        /// <returns>True if the product exists, otherwise false.</returns>
        private bool ProductoExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}

