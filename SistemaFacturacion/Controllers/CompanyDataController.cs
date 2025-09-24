using System.Threading.Tasks;
using SistemaFacturacion.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SistemaFacturacion.Controllers
{
    /// <summary>
    /// Controlador para gestionar la configuración de la empresa emisora de facturas.
    /// Solo los usuarios con el rol de "Administrador" pueden acceder a esta sección.
    /// </summary>
    [Authorize(Roles = "Administrador")]
    public class CompanyDataController : Controller
    {
        private readonly BillingSystemDbContext _context;

        /// <summary>
        /// Constructor que inyecta el contexto de la base de datos.
        /// </summary>
        public CompanyDataController(BillingSystemDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Muestra la vista principal para ver y editar los datos de la empresa.
        /// Carga la única fila de configuración de la base de datos.
        /// GET: /Configuracion/Index
        /// </summary>
        /// <returns>La vista de edición con los datos de la empresa.</returns>
        public async Task<IActionResult> Index()
        {
            // Buscamos la primera (y única) fila de configuración.
            // FirstOrDefaultAsync es ideal aquí, ya que la tabla puede estar vacía inicialmente.
            var configuracion = await _context.CompanyData.FirstOrDefaultAsync();

            // Si no existe ninguna configuración (primera vez que se ejecuta),
            // creamos un nuevo objeto para que la vista no falle.
            if (configuracion == null)
            {
                configuracion = new CompanyData();
                TempData["WarningMessage"] = "No se ha encontrado configuración. Por favor, ingrese los datos de la empresa.";
            }

            return View(configuracion);
        }

        /// <summary>
        /// Procesa la solicitud para guardar los datos de configuración de la empresa.
        /// Actualiza la fila existente o crea una nueva si no existe.
        /// POST: /Configuracion/Index
        /// </summary>
        /// <param name="configuracion">El objeto CompanyData con los datos del formulario.</param>
        /// <returns>La misma vista con un mensaje de éxito o error.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(CompanyData configuracion)
        {
            if (ModelState.IsValid)
            {
                // Si el Id es 0, significa que es un registro nuevo (la primera vez que se guarda).
                if (configuracion.Id == 0)
                {
                    _context.CompanyData.Add(configuracion);
                }
                else // Si el Id no es 0, es un registro existente que debemos actualizar.
                {

                    _context.Update(configuracion);
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Los datos de facturación de la empresa se han actualizado correctamente.";

                return RedirectToAction("Index","Invoice");
            }

            // Si el modelo no es válido, volvemos a mostrar el formulario con los errores.
            TempData["ErrorMessage"] = "Hubo un error al guardar. Por favor, revise los datos.";
            return View(configuracion);
        }
    }
}
