using System.ComponentModel.DataAnnotations;

namespace SistemaFacturacion.Models.ViewModels
{
    /// <summary>
    /// ViewModel que encapsula los datos necesarios para el formulario de registro de un nuevo usuario.
    /// Se utiliza para validar y transportar los datos entre la vista y el controlador de forma segura.
    /// </summary>
    public class UserRegistrationViewModel
    {
        /// <summary>
        /// Name de usuario para el nuevo registro. Debe ser único.
        /// </summary>
        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "El nombre de usuario debe tener entre 4 y 50 caracteres.")]
        [Display(Name = "Name de Usuario")]
        public string NombreUsuario { get; set; }

        /// <summary>
        /// Contraseña para el nuevo usuario.
        /// </summary>
        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Contrasena { get; set; }

        /// <summary>
        /// Campo de confirmación de contraseña. Debe coincidir con el campo Contrasena.
        /// </summary>
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Contraseña")]
        [Compare("Contrasena", ErrorMessage = "La contraseña y la confirmación no coinciden.")]
        public string ConfirmarContrasena { get; set; }

        /// <summary>
        /// Rol asignado al nuevo usuario (ej: Administrador, Vendedor).
        /// </summary>
        [Required(ErrorMessage = "Debe seleccionar un rol.")]
        [Display(Name = "Rol del Usuario")]
        public string Rol { get; set; }
    }
}
