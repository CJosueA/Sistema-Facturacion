using SistemaFacturacion.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SistemaFacturacion.Controllers
{
    /// <summary>
    /// Controlador para la gestión CRUD de los Customers.
    /// </summary>
    [Authorize]
    public class CustomersController : Controller
    {
        private readonly BillingSystemDbContext _context;

        /// <summary>
        /// Constructor del controlador que inyecta el contexto de la base de datos.
        /// </summary>
        /// <param name="context">El contexto de la base de datos.</param>
        public CustomersController(BillingSystemDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Muestra la lista de clientes permitiendo la búsqueda por número de identificación  o nombre.
        /// </summary>
        /// <param name="terminoBusqueda">El texto a buscar en los clientes.</param>
        /// <returns>Una vista con la lista de clientes filtrada.</returns>
        public async Task<IActionResult> Index(string terminoBusqueda)
        {
            ViewData["FiltroActual"] = terminoBusqueda;

            var cliente = from p in _context.Customers
                          select p;

            // Logica para hacer que se muestre el cliente que se busque por ID o Name, insensible a mayúsculas/minúsculas.
            if (!string.IsNullOrEmpty(terminoBusqueda)) {
                var busquedaUpper = terminoBusqueda.ToUpper();

                cliente = cliente.Where(p =>
                    p.NumeroIdentificacion.ToUpper().Contains(busquedaUpper) ||
                    p.NombreCompleto.ToUpper().Contains(busquedaUpper)
                );

            }

            return View(await cliente.ToListAsync());
        }

        /// <summary>
        /// Muestra el formulario para crear un nuevo cliente.
        /// Esta acción es para la vista parcial que se usará en el modal.
        /// </summary>
        public IActionResult CreatePartial()
        {
            return PartialView("_CreateCustomerModalPartial.cshtml");
        }

        /// <summary>
        /// MUESTRA el formulario para crear un nuevo cliente en una página dedicada. (Acción GET)
        /// </summary>
        /// <returns>La vista con el formulario de registro en blanco.</returns>
        public IActionResult Create()
        {
            // Esta acción simplemente prepara y muestra la página del formulario de creación.
            return View();
        }

        /// <summary>
        /// Procesa la solicitud para crear un nuevo cliente.
        /// Puede manejar tanto una solicitud estándar como una solicitud AJAX desde un modal.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Customer cliente)
        {
            // Validar que el número de identificación no exista ya
            if (await _context.Customers.AnyAsync(c => c.NumeroIdentificacion == cliente.NumeroIdentificacion))
            {
                ModelState.AddModelError("NumeroIdentificacion", "Ya existe un cliente con este número de identificación.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(cliente);
                await _context.SaveChangesAsync();

                // Si la solicitud es AJAX (viene del modal), devuelve el nuevo cliente como JSON.
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true, cliente = cliente });
                }

                TempData["SuccessMessage"] = $"Cliente '{cliente.NombreCompleto}' registrado exitosamente.";
                return RedirectToAction(nameof(Index));
            }

            // Si la solicitud AJAX falla la validación, devuelve los errores.
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                var errors = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );
                return Json(new { success = false, errors = errors });
            }

            return View(cliente);
        }

        /// <summary>
        /// Muestra los detalles de un cliente específico en una vista de solo lectura.
        /// </summary>
        /// <param name="id">El ID del cliente a buscar.</param>
        /// <returns>La vista de detalles con la información del cliente, o NotFound si no se encuentra.</returns>
        public async Task<IActionResult> Details(int? id)
        {
            // Valida que el ID no sea nulo.
            if (id == null)
            {
                return NotFound();
            }

            // Busca el cliente en la base de datos de forma asíncrona.
            var cliente = await _context.Customers
                .FirstOrDefaultAsync(m => m.Id == id);

            // Si no se encuentra ningún cliente con ese ID, devuelve un error 404.
            if (cliente == null)
            {
                return NotFound();
            }

            // Si se encuentra, muestra la vista 'Details' y le pasa el objeto 'cliente'.
            return View(cliente);
        }

        /// <summary>
        /// MUESTRA el formulario para editar un cliente existente. (Acción GET)
        /// </summary>
        /// <param name="id">El ID del cliente a editar.</param>
        /// <returns>El formulario de edición con los datos del cliente.</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Customers.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }
            return View(cliente);
        }

        /// <summary>
        /// PROCESA los datos enviados desde el formulario de edición. (Acción POST)
        /// </summary>
        /// <param name="id">El ID del cliente que se está editando.</param>
        /// <param name="cliente">El objeto Cliente con los datos modificados.</param>
        /// <returns>Redirecciona a la lista de clientes si la edición fue exitosa.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NumeroIdentificacion,NombreCompleto,CorreoElectronico,Telefono,Direccion")] Customer cliente)
        {
            // Valida que el ID de la URL coincida con el ID del objeto cliente.
            if (id != cliente.Id)
            {
                return NotFound();
            }

            // Valida que el número de identificación no esté repetido en OTRO cliente.
            if (await _context.Customers.AnyAsync(c => c.NumeroIdentificacion == cliente.NumeroIdentificacion && c.Id != cliente.Id))
            {
                ModelState.AddModelError("NumeroIdentificacion", "Ya existe otro cliente con este número de identificación.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Marca la entidad 'cliente' como modificada y guarda los cambios.
                    _context.Update(cliente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Maneja errores de concurrencia (si otro usuario modificó el registro al mismo tiempo).
                    if (!ClienteExists(cliente.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["SuccessMessage"] = $"Cliente '{cliente.NombreCompleto}' actualizado exitosamente.";
                return RedirectToAction(nameof(Index)); // Redirige a la lista de clientes.
            }
            // Si el modelo no es válido, vuelve a mostrar el formulario con los errores.
            return View(cliente);
        }

        /// <summary>
        /// Método privado para verificar si un cliente existe en la base de datos.
        /// </summary>
        private bool ClienteExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }
    }
}
