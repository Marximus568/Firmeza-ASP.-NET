using System.ComponentModel.DataAnnotations;

namespace AdminDashboardApplication.DTOs.SaleItems
{
    public class CreateSaleItemDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        [Required]
        public int SalesId { get; set; }
    }
}