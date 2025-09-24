using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SistemaFacturacion.Controllers
{
    /// <summary>
    /// Controlador para la página principal o dashboard después del inicio de sesión.
    /// Solo los usuarios autenticados pueden acceder a este controlador.
    /// </summary>
    [Authorize]
    public class DashboardController : Controller
    {
        /// <summary>
        /// Muestra la vista principal del dashboard.
        /// </summary>
        /// <returns>La vista Index del Dashboard.</returns>
        public IActionResult Index()
        {
            // Simplemente devuelve la vista correspondiente.
            return View();
        }
    }
}
