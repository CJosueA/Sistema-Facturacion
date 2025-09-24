using System.ComponentModel.DataAnnotations;

namespace SistemaFacturacion.Models
{
    /// <summary>
    /// Representa la entidad que almacena la información de la empresa emisora de las facturas.
    /// Esta tabla está diseñada para contener una única fila con los datos maestros de la compañía.
    /// </summary>
    public class CompanyData
    {
        /// <summary>
        /// Identificador único de la configuración. Clave primaria.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Name comercial completo de la empresa.
        /// </summary>
        [Required(ErrorMessage = "El nombre de la empresa es obligatorio.")]
        [StringLength(100)]
        public string NombreEmpresa { get; set; }

        /// <summary>
        /// Dirección física de la empresa.
        /// </summary>
        [Required(ErrorMessage = "La dirección es obligatoria.")]
        [StringLength(250)]
        public string Direccion { get; set; }

        /// <summary>
        /// Número de teléfono de contacto principal de la empresa.
        /// </summary>
        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [StringLength(50)]
        public string Telefono { get; set; }

        /// <summary>
        /// Correo electrónico oficial de la empresa.
        /// </summary>
        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [StringLength(100)]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
        public string CorreoElectronico { get; set; }

        /// <summary>
        /// Número de identificación fiscal o cédula jurídica de la empresa.
        /// </summary>
        [Required(ErrorMessage = "La cédula jurídica es obligatoria.")]
        [StringLength(50)]
        public string CedulaJuridica { get; set; }
    }
}

