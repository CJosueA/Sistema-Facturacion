using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaFacturacion.Models;

public partial class Product
{
    /// <summary>
    /// Unique identifier for the product (primary key).
    /// </summary>
    [Key] // Annotation for the Primary Key.
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Annotation for auto-increment.
    public int Id { get; set; }

    /// <summary>
    /// Unique product code, system-generated (e.g., "PROD-00001").
    /// This field is a string for greater flexibility.
    /// </summary>
    [Required(ErrorMessage = "El código es requerido")]
    [Display(Name = "Código")]
    [StringLength(50)]
    public string Code { get; set; } = null!;

    /// <summary>
    /// Descriptive name of the product.
    /// </summary>
    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(75, ErrorMessage = "El nombre no puede exceder 75 caracteres")]
    [Display(Name = "Nombre")]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Unit price of the product.
    /// </summary>
    [Required(ErrorMessage = "El precio es requerido")]
    [Range(0, double.MaxValue, ErrorMessage = "El precio debe ser mayor o igual a 0")]
    [Display(Name = "Precio")]
    public decimal Price { get; set; }

    /// <summary>
    /// Category to which the product belongs.
    /// </summary>
    [Required(ErrorMessage = "La categoría es requerida")]
    [StringLength(50, ErrorMessage = "La categoría no puede exceder 50 caracteres")]
    [Display(Name = "Categoría")]
    public string Category { get; set; } = null!;

    /// <summary>
    /// Current quantity in stock.
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "El stock debe ser mayor o igual a 0")]
    [Display(Name = "Stock")]
    public int Stock { get; set; }

    /// <summary>
    /// Indicates whether the product is active in the system.
    /// </summary>
    [Display(Name = "Activo")]
    public bool IsActive { get; set; }

    /// <summary>
    /// Collection of inventory movements associated with this product.
    /// </summary>
    public virtual ICollection<Movement> Movements { get; set; } = new List<Movement>();
}
