using System.ComponentModel.DataAnnotations;

namespace SistemaFacturacion.Models.ViewModels
{
    /// <summary>
    /// ViewModel para el formulario de inicio de sesión
    /// Contiene las validaciones necesarias para la autenticación
    /// </summary>
    public class LoginViewModel
    {
        /// <summary>
        /// Name de usuario ingresado para la autenticación
        /// Campo requerido con validaciones de longitud
        /// </summary>
        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        [StringLength(75, MinimumLength = 3, ErrorMessage = "El nombre de usuario debe tener entre 3 y 75 caracteres")]
        [Display(Name = "Name de Usuario")]
        public string NombreUsuario { get; set; } = string.Empty;

        /// <summary>
        /// Contraseña ingresada por el usuario
        /// Se valida contra el hash almacenado en la base de datos
        /// </summary>
        [Required(ErrorMessage = "La contraseña es requerida")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Contrasena { get; set; } = string.Empty;

        /// <summary>
        /// Opción para mantener la sesión activa por más tiempo
        /// Implementación opcional para mejorar UX
        /// </summary>
        [Display(Name = "Recordarme")]
        public bool RecordarCredenciales { get; set; } = false;
    }
}
