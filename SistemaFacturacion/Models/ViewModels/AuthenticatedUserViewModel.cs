using System.ComponentModel.DataAnnotations;

namespace SistemaFacturacion.Models.ViewModels
{
    /// <summary>
    /// ViewModel para mostrar información del usuario autenticado
    /// Se utiliza en el header/dashboard después del login exitoso
    /// </summary>
    public class AuthenticatedUserViewModel
    {
        /// <summary>
        /// ID del usuario autenticado
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name de usuario a mostrar en la interfaz
        /// </summary>
        public string NombreUsuario { get; set; } = string.Empty;

        /// <summary>
        /// Rol del usuario para control de acceso básico
        /// </summary>
        public string? Rol { get; set; }

        /// <summary>
        /// Date y hora del último inicio de sesión
        /// </summary>
        public DateTime UltimoAcceso { get; set; } = DateTime.Now;
    }
}
