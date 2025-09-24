using System.Diagnostics;
using SistemaFacturacion.Models;
using SistemaFacturacion.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace SistemaFacturacion.Controllers
{
    /// <summary>
    /// Controlador principal de la aplicaci�n. Gestiona las vistas principales como la p�gina de inicio y la de privacidad.
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// Servicio de logging para registrar informaci�n, advertencias o errores de la aplicaci�n.
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
        /// Devuelve la vista principal o p�gina de inicio de la aplicaci�n.
        /// </summary>
        /// <returns>Un objeto <see cref="IActionResult"/> que renderiza la vista Index.</returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Devuelve la vista de la pol�tica de privacidad de la aplicaci�n.
        /// </summary>
        /// <returns>Un objeto <see cref="IActionResult"/> que renderiza la vista Privacy.</returns>
        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// Muestra una vista de error gen�rica. Esta acci�n se activa cuando ocurre una excepci�n no controlada en la aplicaci�n.
        /// </summary>
        /// <returns>Un objeto <see cref="IActionResult"/> que renderiza la vista Error, pasando un modelo con el identificador de la solicitud actual.</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }

        /// <summary>
        /// Muestra la p�gina de acceso denegado.
        /// Esta p�gina se presenta cuando un usuario autenticado intenta acceder a un recurso
        /// para el cual no tiene los roles de autorizaci�n requeridos.
        /// </summary>
        /// <returns>Una vista que informa al usuario sobre la falta de permisos.</returns>
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}