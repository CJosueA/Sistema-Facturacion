using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaFacturacion.Models;

namespace SistemaFacturacion.Controllers
{
    /// <summary>
    /// Manages inventory movements (product entries and exits).
    /// Requires user authentication for all actions.
    /// </summary>
    [Authorize]
    public class MovementsController : Controller
    {
        // Private field to store the database context instance.
        // It's 'readonly' as it's only assigned in the constructor.
        private readonly BillingSystemDbContext _context;

        //// <summary>
        /// Initializes a new instance of the <see cref="MovementsController"/> class.
        /// Dependency Injection is used to provide the database context.
        /// </summary>
        /// <param name="context">The database context instance for the project.</param>
        public MovementsController(BillingSystemDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Displays the main view for the movements module, which is a product search page.
        /// It allows the user to find and select a product to register a new movement.
        /// </summary>
        /// <param name="searchString">The text entered by the user to search by code, name, or category.</param>
        /// <returns>A view containing the list of products that match the search criteria.</returns>
        public async Task<IActionResult> Index(string searchString)
        {
            // Store the search string to display it back in the search box.
            ViewData["FiltroActual"] = searchString;

            // Start a query to retrieve all products.
            var productsQuery = from p in _context.Products
                                 where p.IsActive == true // Filtro clave: solo se pueden aplicar movimientos a productos activos.
                                 select p;

            // If the user has entered a search string, apply the filter.
            if (!String.IsNullOrEmpty(searchString))
            {
                var searchUpper = searchString.ToUpper(); // Convert to uppercase for a case-insensitive search.
                productsQuery = productsQuery.Where(p =>
                    p.Code.ToString().Contains(searchString) ||
                    p.Name.ToUpper().Contains(searchUpper) ||
                    p.Category.ToUpper().Contains(searchUpper)
                );
            }

            // Execute the query against the database and pass the resulting product list to the view.
            return View(await productsQuery.ToListAsync());
        }

        /// <summary>
        /// Displays the form to create a new movement for a specific product.
        /// This action is triggered after a user selects a product from the Index view.
        /// </summary>
        /// <param name="id">The ID of the selected product.</param>
        /// <returns>The movement creation form view.</returns>
        public async Task<IActionResult> Create(int? id)
        {
            // If no product ID is provided, return a 404 Not Found error.
            if (id == null)
            {
                return NotFound();
            }

            // Find the product in the database using the provided ID.
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                // If the product is not found, return a 404 Not Found error.
                return NotFound();
            }

            // Prepare a new Movement object with initial data.
            var movement = new Movement
            {
                ProductId = product.Id,
                Date = DateOnly.FromDateTime(DateTime.Now) // Set the current date by default.
            };

            // Pass additional data to the view using ViewBag to display product information.
            ViewBag.ProductNombre = product.Name;
            ViewBag.ProductStock = product.Stock;

            return View(movement);
        }

        /// <summary>
        /// Processes the data submitted from the movement creation form.
        /// It validates the data, updates the product's stock, and saves the new movement to the database.
        /// </summary>
        /// <param name="movement">The Movement object containing the form data.</param>
        /// <returns>Redirects to the product list on success, or redisplays the form with errors on failure.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,Type,Quantity,Observation")] Movement movement)
        {
            // Set the date on the server-side for security and consistency.
            movement.Date = DateOnly.FromDateTime(DateTime.Now);
            var product = await _context.Products.FindAsync(movement.ProductId);

            // Validation 1: Ensure the product still exists.
            if (product == null)
            {
                ModelState.AddModelError("ProductId", "El product seleccionado ya no existe.");
            }

            // Validation 2 (Business Rule): If it's an "exit" movement, check for sufficient stock.
            if (movement.Type == "Salida" && product != null && product.Stock < movement.Quantity)
            {
                ModelState.AddModelError("Quantity", $"El stock es insuficiente. Stock actual: {product.Stock}");
            }

            // If all validations (model and business rules) pass...
            if (ModelState.IsValid)
            {
                // Update the product's stock based on the movement type.
                if (movement.Type == "Entrada")
                {
                    product.Stock += movement.Quantity;
                }
                else if (movement.Type == "Salida")
                {
                    product.Stock -= movement.Quantity;
                }

                // Use a transaction to ensure data integrity.
                // Either both changes (product and movement) are saved, or none are.
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        _context.Update(product!); // Update the product with the new stock level.
                        _context.Add(movement);      // Add the new movement record.
                        await _context.SaveChangesAsync(); // Save all changes to the database.
                        await transaction.CommitAsync();   // Commit the transaction.

                        TempData["SuccessMessage"] = "Movimiento registrado con exito.";
                        return RedirectToAction("Index", "Movements");
                    }
                    catch (Exception)
                    {
                        // If any error occurs, roll back all changes.
                        await transaction.RollbackAsync();
                        ModelState.AddModelError("", "Ocurrió un error al guardar los datos. La operación fue revertida.");
                        TempData["ErrorMessage"] = "El movement no pudo ser procesado.";
                    }
                }
            }

            // If the model is not valid, reload the view with the error messages.
            ViewBag.ProductNombre = product?.Name ?? "Producto no encontrado";
            ViewBag.ProductStock = product?.Stock ?? 0;
            return View(movement);
        }

        /// <summary>
        /// Displays the movement history for a specific product.
        /// </summary>
        /// <param name="id">The ID of the product whose history is to be viewed.</param>
        /// <returns>A view with the list of movements associated with the product.</returns>
        public async Task<IActionResult> History(int? id)
        {
            // Validate that an ID has been provided.
            if (id == null)
            {
                return NotFound();
            }

            // Find the product in the database to get its name for display in the view.
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            // Pass the product name to the view via ViewBag.
            ViewBag.ProductNombre = product.Name;

            // Query the Movements table for all records matching the product ID.
            // Order by date descending to show the most recent movements first.
            var movementHistory = await _context.Movements
                .Where(m => m.ProductId == id)
                .OrderByDescending(m => m.Date)
                .ToListAsync();

            // Send the list of movements to the "History" view.
            return View(movementHistory);
        }
    }
}