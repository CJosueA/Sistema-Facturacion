namespace SistemaFacturacion.Models
{
    /// <summary>
    /// ViewModel for the invoice details view.
    /// Contains all the information needed to render the complete invoice,
    /// including the issuing company's information and transaction details.
    /// </summary>
    public class InvoiceDetailsViewModel
    {
        /// <summary>
        /// Contains all invoice information (customer, detail lines, totals).
        /// </summary>
        public Invoice Invoice { get; set; }

        /// <summary>
        /// Contains the data of the company issuing the invoice.
        /// </summary>
        public CompanyData CompanyData { get; set; }
    }
}