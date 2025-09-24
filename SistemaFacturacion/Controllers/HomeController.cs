using System.Diagnostics;
using SistemaFacturacion.Models;
using SistemaFacturacion.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace SistemaFacturacion.Controllers
{
    /// <summary>
    /// Controlador principal de la aplicación. Gestiona las vistas principales como la página de inicio y la de privacidad.
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// Servicio de logging para registrar información, advertencias o errores de la aplicación.
        /// </summary>
        private readonly ILogger<HomeController> _logger;

        /// <summary>
        /// Inicializa una nueva instancia del <see cref="HomeController"/>.
        /// </summary>
        /// <param name="logger">El servicio de logging inyectado por el contenedor de dependencias.</param>
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Devuelve la vista principal o página de inicio de la aplicación.
        /// </summary>
        /// <returns>Un objeto <see cref="IActionResult"/> que renderiza la vista Index.</returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Devuelve la vista de la política de privacidad de la aplicación.
        /// </summary>
        /// <returns>Un objeto <see cref="IActionResult"/> que renderiza la vista Privacy.</returns>
        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// Muestra una vista de error genérica. Esta acción se activa cuando ocurre una excepción no controlada en la aplicación.
        /// </summary>
        /// <returns>Un objeto <see cref="IActionResult"/> que renderiza la vista Error, pasando un modelo con el identificador de la solicitud actual.</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }

        /// <summary>
        /// Muestra la página de acceso denegado.
        /// Esta página se presenta cuando un usuario autenticado intenta acceder a un recurso
        /// para el cual no tiene los roles de autorización requeridos.
        /// </summary>
        /// <returns>Una vista que informa al usuario sobre la falta de permisos.</returns>
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}