using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaFacturacion.Models
{
    /// <summary>
    /// Representa el encabezado de una factura.
    /// Contiene los datos generales de la transacción.
    /// </summary>
    public class Invoice
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Número de Invoice")]
        public string NumeroFactura { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date de Emisión")]
        public DateTime FechaEmision { get; set; }

        [Required]
        public int ClienteId { get; set; }

        [ForeignKey("ClienteId")]
        public virtual Customer Cliente { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Subtotal { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Impuesto { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Total { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Condición de Pago")]
        public string CondicionPago { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date de Vencimiento")]
        public DateTime? FechaVencimiento { get; set; }

        /// <summary>
        /// Propiedad de navegación para el detalle de productos de la factura.
        /// </summary>
        public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();
    }
}
