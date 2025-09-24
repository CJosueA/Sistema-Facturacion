using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace SistemaFacturacion.Models;

/// <summary>
/// Represents an inventory movement, which can be an entry or an exit of a product.
/// </summary>
public partial class Movement
{
    /// <summary>
    /// Gets or sets the unique identifier (primary key) for the movement.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the date on which the inventory movement occurred.
    /// </summary>
    [Required(ErrorMessage = "La fecha es requerida")]
    [Display(Name = "Fecha")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateOnly Date { get; set; }

    /// <summary>
    /// Gets or sets the type of movement. Expected values are "Entrada" or "Salida".
    /// </summary>
    /// <remarks>
    /// For a more robust implementation, consider using an Enum (e.g., MovementType.Entry, MovementType.Exit).
    /// </remarks>
    [Required(ErrorMessage = "El tipo de movimiento es requerido")]
    [StringLength(30, ErrorMessage = "El tipo no puede exceder 30 caracteres")]
    [RegularExpression("^(Entrada|Salida)$", ErrorMessage = "El tipo debe ser 'Entrada' o 'Salida'")]
    [Display(Name = "Tipo")]
    public string Type { get; set; } = null!;

    /// <summary>
    /// Gets or sets the foreign key for the related Product.
    /// </summary>
    [Required(ErrorMessage = "Debe seleccionar un producto")]
    [Display(Name = "Producto")]
    public int ProductId { get; set; }

    /// <summary>
    /// Gets or sets the number of units involved in the movement. Must be a positive integer.
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
    [Display(Name = "Cantidad")]
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets an optional observation or note for the movement.
    /// </summary>
    [StringLength(200, ErrorMessage = "La observación no puede exceder 200 caracteres")]
    [Display(Name = "Observación")]
    public string? Observation { get; set; }

    // --- Navigation Properties ---

    /// <summary>
    /// Navigation property to the associated Product.
    /// </summary>
    [ForeignKey("ProductId")]
    [ValidateNever]
    public virtual Product Product { get; set; } = null!;
}
