using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Authorization;

namespace SistemaFacturacion.Models;

/// <summary>
/// Modelo que representa un usuario del sistema de inventario
/// Contiene la información básica para autenticación y autorización
/// </summary>
[Authorize(Roles = "Administrador")]
public partial class User
{
    /// <summary>
    /// Identificador único del usuario (clave primaria)
    /// </summary>
    [Key] // Le dice a EF que esta es la Clave Primaria
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Le dice a EF que la BD genera el valor
    public int Id { get; set; }

    /// <summary>
    /// Name de usuario único para el inicio de sesión
    /// Debe tener entre 3 y 75 caracteres
    /// </summary>
    [Required(ErrorMessage = "El nombre de usuario es requerido")]
    [StringLength(75, MinimumLength = 3, ErrorMessage = "El nombre de usuario debe tener entre 3 y 75 caracteres")]
    [Display(Name = "Nombre de Usuario")]
    public string NombreUsuario { get; set; } = null!;

    /// <summary>
    /// Contraseña encriptada del usuario
    /// Se almacena como hash por seguridad, no en texto plano
    /// </summary>
    [Required(ErrorMessage = "La contraseña es requerida")]
    [StringLength(255, ErrorMessage = "La contraseña no puede exceder 255 caracteres")]
    [Display(Name = "Contraseña")]
    public string HashContrasena { get; set; } = null!;

    /// <summary>
    /// Rol del usuario en el sistema (Administrador, Usuario, etc.)
    /// Campo opcional para control de acceso básico
    /// </summary>
    [StringLength(30, ErrorMessage = "El rol no puede exceder 30 caracteres")]
    [Display(Name = "Rol")]
    public string? Rol { get; set; }
}
