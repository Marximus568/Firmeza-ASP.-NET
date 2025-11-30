using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdminDashboard.Domain.Entities;
using AdminDashboard.Infrastructure.Persistence.Context;
using AdminDashboardApplication.DTOs.SaleDate;
using AdminDashboardApplication.DTOs.Sales;
using AdminDashboardApplication.Interfaces;
using Firmeza.WebApi.wwwroot.receipt;
using Swashbuckle.AspNetCore.Annotations;

namespace Firmeza.WebApi.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class SalesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IPdfService _pdfService;
        private readonly IEmailService _emailService;

        public SalesController(AppDbContext context, IMapper mapper, IPdfService pdfService, IEmailService emailService)
        {
            _context = context;
            _mapper = mapper;
            _pdfService = pdfService;
            _emailService = emailService;
        }

        // =========================================================
        // POST: v1/sales/register-sale
        // =========================================================
        /// <summary>
        /// Registers a sale and automatically generates a PDF receipt.
        /// </summary>
        /// <param name="dto">Sale receipt data including customer and products.</param>
        /// <returns>A URL for downloading the generated PDF.</returns>
        /// <response code="200">Sale registered and PDF generated successfully.</response>
        /// <response code="400">Invalid sale data.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPost("register-sale")]
        [SwaggerOperation(
            Summary = "Register a sale and generate PDF receipt",
            Description = "Creates a sale entry in the system and generates a PDF receipt stored in wwwroot/receipts.",
            OperationId = "RegisterSale",
            Tags = new[] { "Sales" }
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(object))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid sale data")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
        public async Task<IActionResult> RegisterSale([FromBody] SaleReceiptDto dto)
        {
            if (dto == null)
                return BadRequest(new { message = "Invalid sale data." });

            // 1. Find or Create Client
            var client = await _context.Users.FirstOrDefaultAsync(c => c.Email == dto.CustomerEmail);
            if (client == null)
            {
                // Split name into First/Last if possible
                var names = dto.CustomerName.Split(' ', 2);
                var firstName = names[0];
                var lastName = names.Length > 1 ? names[1] : ".";

                client = new Clients
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = dto.CustomerEmail,
                    PhoneNumber = "0000000000", // Default
                    Address = "Online Customer", // Default
                    Role = "Client",
                    Password = "", // No password for guest checkout
                    DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow) // Default valid date
                };
                _context.Users.Add(client);
                await _context.SaveChangesAsync();
            }

            // 2. Save sale in DB
            var saleEntity = _mapper.Map<Sales>(dto);
            saleEntity.ClientId = client.Id; // Assign the valid ClientId
            
            _context.Sales.Add(saleEntity);
            await _context.SaveChangesAsync();

            // 3. Generate PDF
            string pdfUrl = _pdfService.GenerateReceiptPdf(dto);

            // 4. Send email with PDF attachment
            try
            {
                // Extract filename from URL
                var pdfFilename = pdfUrl.Split('/').Last();
                var pdfPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "receipts", pdfFilename);

                // Read PDF file as bytes
                byte[] pdfBytes = await System.IO.File.ReadAllBytesAsync(pdfPath);

                // Create email body
                var emailBody = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif;'>
                        <h2>¡Gracias por tu compra!</h2>
                        <p>Hola <strong>{dto.CustomerName}</strong>,</p>
                        <p>Tu pedido ha sido procesado exitosamente.</p>
                        <h3>Resumen de la compra:</h3>
                        <table style='border-collapse: collapse; width: 100%;'>
                            <thead>
                                <tr style='background-color: #f2f2f2;'>
                                    <th style='border: 1px solid #ddd; padding: 8px; text-align: left;'>Producto</th>
                                    <th style='border: 1px solid #ddd; padding: 8px; text-align: right;'>Cantidad</th>
                                    <th style='border: 1px solid #ddd; padding: 8px; text-align: right;'>Precio</th>
                                </tr>
                            </thead>
                            <tbody>
                                {string.Join("", dto.Products.Select(p => $@"
                                <tr>
                                    <td style='border: 1px solid #ddd; padding: 8px;'>{p.Name}</td>
                                    <td style='border: 1px solid #ddd; padding: 8px; text-align: right;'>{p.Qty}</td>
                                    <td style='border: 1px solid #ddd; padding: 8px; text-align: right;'>${p.UnitPrice:F2}</td>
                                </tr>"))}
                            </tbody>
                        </table>
                        <p style='margin-top: 20px;'>
                            <strong>Subtotal:</strong> ${dto.Subtotal:F2}<br/>
                            <strong>IVA (16%):</strong> ${dto.Iva:F2}<br/>
                            <strong>Total:</strong> ${dto.Total:F2}
                        </p>
                        <p>Adjunto encontrarás el comprobante de compra en formato PDF.</p>
                        <p>¡Gracias por tu preferencia!</p>
                    </body>
                    </html>";

                // Send email
                await _emailService.SendEmailWithAttachmentAsync(
                    dto.CustomerEmail,
                    "Confirmación de Compra - Firmeza",
                    emailBody,
                    pdfBytes,
                    pdfFilename
                );
            }
            catch (Exception ex)
            {
                // Log error but don't fail the request
                Console.WriteLine($"Failed to send email: {ex.Message}");
                Console.WriteLine($"Exception details: {ex}");
            }

            return Ok(new
            {
                message = "Sale registered successfully",
                pdf = pdfUrl
            });
        }

        // =========================================================
        // GET: v1/sales/download
        // =========================================================
        /// <summary>
        /// Downloads a generated PDF receipt from the server.
        /// </summary>
        /// <param name="file">The file name to download.</param>
        /// <returns>A PDF file.</returns>
        [HttpGet("download")]
        [SwaggerOperation(
            Summary = "Download PDF receipt",
            Description = "Returns a previously generated receipt located in wwwroot/receipts.",
            OperationId = "DownloadReceipt",
            Tags = new[] { "Sales" }
        )]
        public IActionResult Download([FromQuery] string file)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "receipts", file);

            if (!System.IO.File.Exists(filePath))
                return NotFound(new { message = "File not found." });

            return PhysicalFile(filePath, "application/pdf", file);
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SaleDto>>> GetSales()
        {
            var sales = await _context.Sales.ToListAsync();
            var salesDto = _mapper.Map<List<SaleDto>>(sales);
            return Ok(salesDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SaleDto>> GetSale(int id)
        {
            var sale = await _context.Sales.FindAsync(id);
            if (sale == null)
                return NotFound(new { message = $"Sale with ID {id} not found." });

            var saleDto = _mapper.Map<SaleDto>(sale);
            return Ok(saleDto);
        }

        [HttpPost]
        public async Task<ActionResult<SaleDto>> PostSale([FromBody] CreateSaleDto dto)
        {
            var entity = _mapper.Map<Sales>(dto);
            _context.Sales.Add(entity);
            await _context.SaveChangesAsync();

            var result = _mapper.Map<SaleDto>(entity);
            return CreatedAtAction(nameof(GetSale), new { id = entity.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutSale(int id, [FromBody] CreateSaleDto dto)
        {
            var entity = await _context.Sales.FindAsync(id);
            if (entity == null)
                return NotFound(new { message = $"Sale with ID {id} not found." });

            _mapper.Map(dto, entity);
            _context.Entry(entity).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSale(int id)
        {
            var entity = await _context.Sales.FindAsync(id);
            if (entity == null)
                return NotFound(new { message = $"Sale with ID {id} not found." });

            _context.Sales.Remove(entity);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
