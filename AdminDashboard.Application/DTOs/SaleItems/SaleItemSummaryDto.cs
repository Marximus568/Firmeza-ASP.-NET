namespace AdminDashboardApplication.DTOs.SaleItems
{
    /// <summary>
    /// Represents a summarized view of a product sold in a sale transaction.
    /// Used inside SaleResponseDto to show sale details without exposing full product entity.
    /// </summary>
    public class SaleItemSummaryDto
    {
        /// <summary>
        /// Unique identifier of the sale item.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identifier of the related product.
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Product name (retrieved from the related entity).
        /// </summary>
        public string ProductName { get; set; } = string.Empty;

        /// <summary>
        /// Quantity of product sold in this sale.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Unit price at the time of sale.
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Subtotal for this item (Quantity × UnitPrice).
        /// </summary>
        public decimal Subtotal => Quantity * UnitPrice;
    }
}