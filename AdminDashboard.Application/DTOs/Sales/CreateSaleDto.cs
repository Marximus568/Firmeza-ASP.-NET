using AdminDashboard.Application.DTOs.Sales;

namespace AdminDashboardApplication.DTOs.Sales;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class CreateSaleDto
{
    [Required]
    public int ClientId { get; set; }

    [Required]
    public decimal TaxRate { get; set; }

    [Required]
    public string PaymentMethod { get; set; } = string.Empty;

    public string Notes { get; set; } = string.Empty;

    // Items to be included in the sale
    [Required]
    public List<SaleSummaryDto> Items { get; set; } = new();
}
