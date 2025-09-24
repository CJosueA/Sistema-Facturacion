using System.ComponentModel.DataAnnotations;

namespace SistemaFacturacion.Models
{
    /// <summary>
    /// Representa a un cliente en la base de datos.
    /// Contiene toda la información de contacto y fiscal del cliente.
    /// </summary>
    public class Customer
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre completo es requerido.")]
        [StringLength(150)]
        [Display(Name = "Nombre completo o Razón social")]
        public string NombreCompleto { get; set; }

        [Required(ErrorMessage = "El número de identificación es requerido.")]
        [StringLength(50)]
        [Display(Name = "Número de identificación (Cédula/NIT/RFC)")]
        public string NumeroIdentificacion { get; set; }

        [StringLength(250)]
        [Display(Name = "Dirección")]
        public string Direccion { get; set; }

        [StringLength(20)]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; }

        [StringLength(100)]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
        [Display(Name = "Correo electrónico")]
        public string CorreoElectronico { get; set; }

        /// <summary>
        /// Propiedad de navegación para las facturas asociadas a este cliente.
        /// </summary>
        public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}
