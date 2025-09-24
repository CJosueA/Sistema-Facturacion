using System.Text.Json;
using SistemaFacturacion.Models;
using SistemaFacturacion.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SistemaFacturacion.Controllers
{
    /// <summary>
    /// Controlador para gestionar todo el proceso de facturación.
    /// Solo los usuarios autenticados pueden acceder.
    /// </summary>
    [Authorize]
    public class InvoiceController : Controller
    {
        private readonly BillingSystemDbContext _context;

        public InvoiceController(BillingSystemDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Muestra una lista de todas las facturas emitidas.
        /// También permite la busqueda de una factura especifíca por su numero o por el nombre del cliente
        /// </summary>
        public async Task<IActionResult> Index(string terminoBusqueda)
        {
            ViewData["FiltroActual"] = terminoBusqueda;

            var facturaQuery = from p in _context.Invoices
                          select p;

            if (!string.IsNullOrEmpty(terminoBusqueda)) {
                var busquedaUpper = terminoBusqueda.ToUpper();

                facturaQuery = facturaQuery.Where(p =>
                    p.NumeroFactura.Contains(busquedaUpper) ||
                    p.Cliente.NombreCompleto.ToUpper().Contains(busquedaUpper)
                );
            }

            return View(await facturaQuery.Include(f => f.Cliente).OrderByDescending(f => f.FechaEmision).ToListAsync());
        }

        /// <summary>
        /// Muestra el formulario para crear una nueva factura.
        /// </summary>
        public IActionResult Create()
        {
            // Podríamos precargar clientes si es necesario, pero lo haremos con búsqueda
            return View();
        }

        // POST: Facturacion/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("ClienteId,CondicionPago")] Invoice factura,
            string detallesJson)
        {
            // Inicia una transacción para asegurar la integridad de los datos.
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                /// <summary>
                /// Deserializa los datos de entrada usando el nuevo ViewModel.
                /// </summary>
                var detallesDesdeCliente = JsonSerializer.Deserialize<List<InvoiceDetailApiViewModel>>(detallesJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (detallesDesdeCliente == null || !detallesDesdeCliente.Any())
                {
                    ModelState.AddModelError("", "Debe agregar al menos un producto a la factura.");
                    return View(factura);
                }

                /// <summary>
                /// Inicializa variables para los cálculos de la factura.
                /// </summary>
                decimal subtotalFactura = 0;
                var detallesParaGuardar = new List<InvoiceDetail>();

                /// <summary>
                /// Procesa cada línea de detalle en el servidor.
                /// </summary>
                foreach (var detalleCliente in detallesDesdeCliente)
                {
                    var productoEnDB = await _context.Products.FindAsync(detalleCliente.ProductId);

                    // Validar existencia y stock del producto.
                    if (productoEnDB == null || productoEnDB.Stock < detalleCliente.Quantity)
                    {
                        await transaction.RollbackAsync();
                        ModelState.AddModelError("", $"Stock insuficiente para el producto '{productoEnDB?.Name ?? "Desconocido"}'. Disponible: {productoEnDB?.Stock}.");
                        return View(factura);
                    }

                    // Disminuir el stock.
                    productoEnDB.Stock -= detalleCliente.Quantity;
                    _context.Update(productoEnDB);

                    // Create el objeto FacturaDetalle completo con datos del servidor.
                    var nuevoDetalle = new InvoiceDetail
                    {
                        ProductId = detalleCliente.ProductId,
                        Quantity = detalleCliente.Quantity,
                        UnitPrice = productoEnDB.Price, // ¡Price obtenido de la base de datos!
                        Subtotal = detalleCliente.Quantity * productoEnDB.Price // ¡Cálculo hecho en el servidor!
                    };

                    detallesParaGuardar.Add(nuevoDetalle);
                    subtotalFactura += nuevoDetalle.Subtotal; // Acumular el subtotal de la factura.

                    // Create el movimiento de inventario.
                    var movimientoDeVenta = new Movement
                    {
                        ProductId = productoEnDB.Id,
                        Type = "Salida",
                        Quantity = detalleCliente.Quantity,
                        Date = DateOnly.FromDateTime(DateTime.Now),
                        Observation = $"Venta - Factura Nº {factura.NumeroFactura}" // El número se asignará después
                    };
                    _context.Movements.Add(movimientoDeVenta);
                }

                /// <summary>
                /// Calcula totales y asigna todos los datos a la factura principal.
                /// </summary>
                factura.FechaEmision = DateTime.Now;
                factura.NumeroFactura = $"F-{DateTime.Now.Ticks}";
                factura.Subtotal = subtotalFactura;
                factura.Impuesto = subtotalFactura * 0.13m; // 13% IVA.
                factura.Total = factura.Subtotal + factura.Impuesto;
                factura.InvoiceDetails = detallesParaGuardar;

                // Actualizar la observación del movimiento con el número de factura final.
                foreach (var movimiento in _context.ChangeTracker.Entries<Movement>().Select(e => e.Entity))
                {
                    movimiento.Observation = $"Venta - Factura Nº {factura.NumeroFactura}";
                }

                /// <summary>
                /// Guarda todo en la base de datos.
                /// </summary>
                _context.Invoices.Add(factura);
                await _context.SaveChangesAsync();

                // Confirmar la transacción.
                await transaction.CommitAsync();
                TempData["SuccessMessage"] = $"Factura {factura.NumeroFactura} creada exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                // Opcional: Registrar el error 'ex' en un sistema de logs.
                ModelState.AddModelError("", "Ocurrió un error inesperado al guardar la factura. Por favor, intente de nuevo.");
                return View(factura);
            }
        }

        /// <summary>
        /// API endpoint para buscar productos por término de búsqueda.
        /// Devuelve un JSON con los productos que coinciden.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> BuscarProductos(string term)
        {
            if (string.IsNullOrEmpty(term))
            {
                return Json(new List<Product>());
            }

            // Convertimos el término de búsqueda a mayúsculas una sola vez.
            var termUpper = term.ToUpper();

            var productos = await _context.Products
                // Comparamos todo en mayúsculas.
                .Where(p => p.IsActive &&
                            (p.Name.ToUpper().Contains(termUpper) || p.Code.ToUpper().Contains(termUpper)))
                .Select(p => new {
                    id = p.Id,
                    label = $"{p.Name} ({p.Code})",
                    value = p.Name,
                    codigo = p.Code,
                    precio = p.Price,
                    stock = p.Stock
                })
                .Take(10)
                .ToListAsync();

            return Json(productos);
        }

        /// <summary>
        /// API endpoint para buscar clientes por término de búsqueda.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> BuscarClientes(string term)
        {
            if (string.IsNullOrEmpty(term))
            {
                return Json(new List<Customer>());
            }

            // Convertimos el término de búsqueda a mayúsculas.
            var termUpper = term.ToUpper();

            var clientes = await _context.Customers
                // Comparamos los campos relevantes en mayúsculas.
                .Where(c => c.NombreCompleto.ToUpper().Contains(termUpper) ||
                            c.NumeroIdentificacion.ToUpper().Contains(termUpper))
                 .Select(c => new {
                     id = c.Id,
                     label = $"{c.NombreCompleto} ({c.NumeroIdentificacion})",
                     value = c.NombreCompleto,
                     identificacion = c.NumeroIdentificacion,
                     direccion = c.Direccion,
                     telefono = c.Telefono,
                     email = c.CorreoElectronico
                 })
                .Take(10)
                .ToListAsync();

            return Json(clientes);
        }

        /// <summary>
        /// Muestra una vista detallada de una factura específica, incluyendo los datos de la empresa emisora.
        /// </summary>
        /// <param name="id">El ID de la factura que se desea consultar.</param>
        /// <returns>Una vista con la información completa de la factura.</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                // --- PASO 1: Buscar la información de la factura (como antes) ---
                var factura = await _context.Invoices
                    .Include(f => f.Cliente)
                    .Include(f => f.InvoiceDetails)
                        .ThenInclude(d => d.Product)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (factura == null)
                {
                    return NotFound();
                }

                // --- PASO 2: Buscar la configuración de la empresa ---
                // Usamos FirstOrDefaultAsync asumiendo que solo habrá una fila de configuración.
                var empresa = await _context.CompanyData.FirstOrDefaultAsync();
                if (empresa == null)
                {
                    // Manejo de error por si la tabla de configuración está vacía.
                    // Podrías redirigir a una página para que configuren los datos.
                    // Por ahora, retornamos un error o una vista con un mensaje.
                    return View("Error", new ErrorViewModel { Message = "Los datos de la empresa no están configurados." });
                }

                // --- PASO 3: Create y poblar el ViewModel ---
                var viewModel = new InvoiceDetailsViewModel
                {
                    Invoice = factura,
                    CompanyData = empresa
                };

                // --- PASO 4: Enviar el ViewModel a la vista ---
                return View(viewModel);
            }
            catch (Exception ex) {
                // Este bloque capturará el error exacto y lo mostrará.
                // La información más valiosa casi siempre está en la "InnerException".
                var errorMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    errorMessage += " ---> INNER EXCEPTION: " + ex.InnerException.Message;
                }

                // Retornamos una vista de error con el mensaje detallado.
                return View("Error", new ErrorViewModel { Message = "ERROR DE CARGA DE DATOS: " + errorMessage });
            }

        }

        /// <summary>
        /// Genera y descarga una versión en PDF de una factura específica.
        /// </summary>
        /// <param name="id">El ID de la factura a convertir en PDF.</param>
        /// <returns>Un archivo PDF para descargar.</returns>
        public async Task<IActionResult> DescargarPdf(int id)
        {
            // 1. Obtenemos los datos de la misma forma que en el método 'Details'
            var factura = await _context.Invoices
                .Include(f => f.Cliente)
                .Include(f => f.InvoiceDetails)
                    .ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (factura == null) return NotFound();

            var empresa = await _context.CompanyData.FirstOrDefaultAsync();
            if (empresa == null) return NotFound();

            var viewModel = new InvoiceDetailsViewModel
            {
                Invoice = factura,
                CompanyData = empresa
            };

            // 2. Usamos Rotativa para convertir la vista 'InvoicePdf' en un PDF
            return new Rotativa.AspNetCore.ViewAsPdf("InvoicePdf", viewModel)
            {
                // Opcional: define el nombre del archivo PDF que se descargará
                FileName = $"Factura-{viewModel.Invoice.NumeroFactura}.pdf"
            };
        }
    }
}
