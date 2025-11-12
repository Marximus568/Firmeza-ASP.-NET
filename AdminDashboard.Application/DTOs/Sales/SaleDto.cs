namespace AdminDashboardApplication.DTOs.Sales
{
    /// <summary>
    /// Represents a basic sale data structure shared between layers.
    /// This DTO can be mapped bi-directionally using AutoMapper.
    /// </summary>
    public class SaleDto
    {
        /// <summary>
        /// Unique identifier of the sale.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Date when the sale occurred (in UTC format).
        /// </summary>
        public DateTime SaleDate { get; set; }

        /// <summary>
        /// Invoice number generated for the sale.
        /// </summary>
        public string InvoiceNumber { get; set; } = string.Empty;

        /// <summary>
        /// Identifier of the related client.
        /// </summary>
        public int ClientId { get; set; }

        /// <summary>
        /// Identifier of the employee or seller who made the sale.
        /// </summary>
        public int SalerId { get; set; }

        /// <summary>
        /// Subtotal amount before tax.
        /// </summary>
        public decimal Subtotal { get; set; }

        /// <summary>
        /// Applied tax rate (e.g., 0.19 = 19%).
        /// </summary>
        public decimal TaxRate { get; set; }

        /// <summary>
        /// Total amount to be paid (Subtotal + Tax).
        /// </summary>
        public decimal Total { get; set; }

        /// <summary>
        /// Payment method used by the client.
        /// </summary>
        public string PaymentMethod { get; set; } = string.Empty;

        /// <summary>
        /// Indicates if the sale has been fully paid.
        /// </summary>
        public bool IsPaid { get; set; }

        /// <summary>
        /// Optional notes related to the sale.
        /// </summary>
        public string Notes { get; set; } = string.Empty;
    }
}