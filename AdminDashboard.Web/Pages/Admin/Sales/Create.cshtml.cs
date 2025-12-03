using AdminDashboard.Infrastructure.Persistence.Context;
using AdminDashboard.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AdminDashboard.Application.Interfaces.SalePDF;
using AdminDashboard.Application.DTOs.SalePDF;
using AdminDashboardApplication.Interfaces;

namespace AdminDashboard.Pages.Admin.Sales;

public class CreateModel : PageModel
{
    private readonly AppDbContext _context;
    private readonly IPdfService _pdfService;
    private readonly IWebHostEnvironment _env;
    private readonly IEmailService _emailService;
    private readonly ILogger<CreateModel> _logger;

    public CreateModel(
        AppDbContext context, 
        IPdfService pdfService, 
        IWebHostEnvironment env,
        IEmailService emailService,
        ILogger<CreateModel> logger)
    {
        _context = context;
        _pdfService = pdfService;
        _env = env;
        _emailService = emailService;
        _logger = logger;
    }

    public SelectList Clients { get; set; } = default!;

    [BindProperty]
    public AdminDashboard.Domain.Entities.Sales Sale { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var clients = await _context.Users.ToListAsync();
        Clients = new SelectList(clients, "Id", "FirstName");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            var clients = await _context.Users.ToListAsync();
            Clients = new SelectList(clients, "Id", "FirstName");
            return Page();
        }

        _context.Sales.Add(Sale);
        await _context.SaveChangesAsync();

        // Generate PDF receipt and send email
        var emailSent = await GenerateReceiptPdfAsync(Sale.Id);

        if (emailSent)
        {
            TempData["SuccessMessage"] = $"Sale '{Sale.InvoiceNumber}' created successfully! Receipt generated and sent to customer's email.";
            TempData["EmailSent"] = true;
        }
        else
        {
            TempData["SuccessMessage"] = $"Sale '{Sale.InvoiceNumber}' created successfully and receipt generated. Email could not be sent.";
            TempData["EmailSent"] = false;
        }

        return RedirectToPage("./Index");
    }

    private async Task<bool> GenerateReceiptPdfAsync(int saleId)
    {
        var emailSent = false;
        
        try
        {
            // Get sale with all necessary data
            var sale = await _context.Sales
                .Include(s => s.Clients)
                .Include(s => s.Items)
                    .ThenInclude(i => i.Products)
                .FirstOrDefaultAsync(s => s.Id == saleId);

            if (sale == null) return false;

            // Prepare DTO for PDF generation
            var receiptData = new SaleReceiptDto
            {
                SaleId = sale.Id,
                InvoiceNumber = sale.InvoiceNumber,
                Date = sale.SaleDate,
                CustomerName = $"{sale.Clients.FirstName} {sale.Clients.LastName}",
                CustomerEmail = sale.Clients.Email,
                Products = sale.Items.Select(item => new SaleProductDto
                {
                    Name = item.Products.Name,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                }).ToList(),
                Subtotal = sale.Subtotal,
                Iva = sale.Total - sale.Subtotal,
                Total = sale.Total
            };

            // Generate PDF bytes
            var pdfBytes = _pdfService.GenerateReceiptPdfBytes(receiptData);

            // Create recibos folder if it doesn't exist
            var receiptsPath = Path.Combine(_env.WebRootPath, "recibos");
            Directory.CreateDirectory(receiptsPath);

            // Save PDF file
            var fileName = $"Recibo_{sale.InvoiceNumber.Replace("/", "-")}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            var filePath = Path.Combine(receiptsPath, fileName);
            await System.IO.File.WriteAllBytesAsync(filePath, pdfBytes);

            // Update sale with PDF path
            sale.ReceiptPath = $"/recibos/{fileName}";
            await _context.SaveChangesAsync();

            // Send email with PDF attachment
            try
            {
                var emailSubject = $"Receipt for Invoice {sale.InvoiceNumber}";
                var emailBody = CreateEmailTemplate(receiptData);

                await _emailService.SendEmailWithAttachmentAsync(
                    sale.Clients.Email,
                    emailSubject,
                    emailBody,
                    pdfBytes,
                    fileName
                );

                emailSent = true;
                _logger.LogInformation("Receipt email sent successfully to {Email} for sale {SaleId}", 
                    sale.Clients.Email, saleId);
            }
            catch (Exception emailEx)
            {
                _logger.LogError(emailEx, "Failed to send receipt email to {Email} for sale {SaleId}", 
                    sale.Clients.Email, saleId);
                // Don't throw - email failure shouldn't break the sale creation
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating PDF receipt for sale {SaleId}", saleId);
        }

        return emailSent;
    }

    private string CreateEmailTemplate(SaleReceiptDto receiptData)
    {
        var productsHtml = string.Join("", receiptData.Products.Select(p => $@"
            <tr>
                <td style='padding: 12px; border-bottom: 1px solid #e5e7eb;'>{p.Name}</td>
                <td style='padding: 12px; border-bottom: 1px solid #e5e7eb; text-align: center;'>{p.Quantity}</td>
                <td style='padding: 12px; border-bottom: 1px solid #e5e7eb; text-align: right;'>${p.UnitPrice:F2}</td>
                <td style='padding: 12px; border-bottom: 1px solid #e5e7eb; text-align: right; font-weight: 600;'>${p.Quantity * p.UnitPrice:F2}</td>
            </tr>
        "));

        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
</head>
<body style='margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: #f3f4f6;'>
    <div style='max-width: 600px; margin: 0 auto; background-color: #ffffff;'>
        <!-- Header -->
        <div style='background: linear-gradient(135deg, #3b82f6 0%, #8b5cf6 100%); padding: 40px 30px; text-align: center;'>
            <h1 style='color: #ffffff; margin: 0; font-size: 28px; font-weight: bold;'>
                <span style='font-size: 32px;'>🛡️</span> Firmeza Admin
            </h1>
            <p style='color: #e0e7ff; margin: 10px 0 0 0; font-size: 16px;'>Purchase Receipt</p>
        </div>

        <!-- Content -->
        <div style='padding: 40px 30px;'>
            <h2 style='color: #1f2937; margin: 0 0 20px 0; font-size: 24px;'>Hello {receiptData.CustomerName}!</h2>
            
            <p style='color: #4b5563; line-height: 1.6; margin: 0 0 30px 0;'>
                Thank you for your purchase. Please find attached your receipt for invoice <strong>{receiptData.InvoiceNumber}</strong>.
            </p>

            <!-- Sale Details -->
            <div style='background-color: #f9fafb; border-left: 4px solid #3b82f6; padding: 20px; margin-bottom: 30px; border-radius: 4px;'>
                <h3 style='color: #1f2937; margin: 0 0 15px 0; font-size: 18px;'>Sale Details</h3>
                <table style='width: 100%; border-collapse: collapse;'>
                    <tr>
                        <td style='padding: 8px 0; color: #6b7280;'>Invoice Number:</td>
                        <td style='padding: 8px 0; color: #1f2937; font-weight: 600; text-align: right;'>{receiptData.InvoiceNumber}</td>
                    </tr>
                    <tr>
                        <td style='padding: 8px 0; color: #6b7280;'>Date:</td>
                        <td style='padding: 8px 0; color: #1f2937; font-weight: 600; text-align: right;'>{receiptData.Date:MMMM dd, yyyy}</td>
                    </tr>
                    <tr>
                        <td style='padding: 8px 0; color: #6b7280;'>Total Amount:</td>
                        <td style='padding: 8px 0; color: #059669; font-weight: 700; font-size: 20px; text-align: right;'>${receiptData.Total:F2}</td>
                    </tr>
                </table>
            </div>

            <!-- Products Table -->
            <h3 style='color: #1f2937; margin: 0 0 15px 0; font-size: 18px;'>Products</h3>
            <table style='width: 100%; border-collapse: collapse; margin-bottom: 30px;'>
                <thead>
                    <tr style='background-color: #f3f4f6;'>
                        <th style='padding: 12px; text-align: left; color: #374151; font-weight: 600; border-bottom: 2px solid #e5e7eb;'>Product</th>
                        <th style='padding: 12px; text-align: center; color: #374151; font-weight: 600; border-bottom: 2px solid #e5e7eb;'>Qty</th>
                        <th style='padding: 12px; text-align: right; color: #374151; font-weight: 600; border-bottom: 2px solid #e5e7eb;'>Price</th>
                        <th style='padding: 12px; text-align: right; color: #374151; font-weight: 600; border-bottom: 2px solid #e5e7eb;'>Total</th>
                    </tr>
                </thead>
                <tbody>
                    {productsHtml}
                </tbody>
                <tfoot>
                    <tr>
                        <td colspan='3' style='padding: 12px; text-align: right; color: #6b7280; font-weight: 600;'>Subtotal:</td>
                        <td style='padding: 12px; text-align: right; font-weight: 600;'>${receiptData.Subtotal:F2}</td>
                    </tr>
                    <tr>
                        <td colspan='3' style='padding: 12px; text-align: right; color: #6b7280; font-weight: 600;'>IVA:</td>
                        <td style='padding: 12px; text-align: right; font-weight: 600;'>${receiptData.Iva:F2}</td>
                    </tr>
                    <tr style='background-color: #f3f4f6;'>
                        <td colspan='3' style='padding: 12px; text-align: right; color: #1f2937; font-weight: 700; font-size: 16px;'>Total:</td>
                        <td style='padding: 12px; text-align: right; color: #059669; font-weight: 700; font-size: 18px;'>${receiptData.Total:F2}</td>
                    </tr>
                </tfoot>
            </table>

            <!-- Attachment Note -->
            <div style='background-color: #eff6ff; border: 1px solid #bfdbfe; padding: 15px; border-radius: 4px; margin-bottom: 30px;'>
                <p style='color: #1e40af; margin: 0; font-size: 14px;'>
                    📎 <strong>Attached:</strong> Your detailed receipt is attached to this email as a PDF file.
                </p>
            </div>

            <p style='color: #4b5563; line-height: 1.6; margin: 0;'>
                If you have any questions about this purchase, please don't hesitate to contact us.
            </p>

            <p style='color: #4b5563; line-height: 1.6; margin: 20px 0 0 0;'>
                Best regards,<br>
                <strong>Firmeza Admin Team</strong>
            </p>
        </div>

        <!-- Footer -->
        <div style='background-color: #f9fafb; padding: 30px; text-align: center; border-top: 1px solid #e5e7eb;'>
            <p style='color: #6b7280; margin: 0; font-size: 14px;'>
                © {DateTime.Now.Year} Firmeza Admin. All rights reserved.
            </p>
            <p style='color: #9ca3af; margin: 10px 0 0 0; font-size: 12px;'>
                This is an automated email. Please do not reply to this message.
            </p>
        </div>
    </div>
</body>
</html>";
    }
}
