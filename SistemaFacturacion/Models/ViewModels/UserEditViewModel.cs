using System.ComponentModel.DataAnnotations;

namespace SistemaFacturacion.Models.ViewModels
{
    /// <summary>
    /// ViewModel para el proceso de edición de un usuario.
    /// Maneja la actualización de datos y el cambio opcional de contraseña.
    /// </summary>
    public class UserEditViewModel
    {
        /// <summary>
        /// ID del usuario.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name de usuario para el inicio de sesión.
        /// </summary>
        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        [Display(Name = "Name de Usuario")]
        public string NombreUsuario { get; set; }

        /// <summary>
        /// Rol asignado al usuario (ej. "Administrador", "Vendedor").
        /// </summary>
        [Required(ErrorMessage = "El rol es obligatorio.")]
        public string Rol { get; set; }

        /// <summary>
        /// Campo opcional para establecer una nueva contraseña.
        /// </summary>
        [DataType(DataType.Password)]
        [Display(Name = "Nueva Contraseña (opcional)")]
        public string? NuevaContrasena { get; set; }

        /// <summary>
        /// Campo de confirmación para la nueva contraseña.
        /// Debe coincidir con NuevaContrasena.
        /// </summary>
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Nueva Contraseña")]
        [Compare("NuevaContrasena", ErrorMessage = "Las contraseñas no coinciden.")]
        public string? ConfirmarContrasena { get; set; }
    }
}
