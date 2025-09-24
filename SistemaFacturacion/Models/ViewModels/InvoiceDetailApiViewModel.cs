namespace SistemaFacturacion.Models.ViewModels
{
    /// <summary>
    /// Representa la información mínima necesaria que el cliente envía
    /// para agregar un producto a la factura.
    /// </summary>
    public class InvoiceDetailApiViewModel
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}