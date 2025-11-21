using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdminDashboard.Domain.Entities;
using AdminDashboard.Infrastructure.Persistence.Context;
using AdminDashboardApplication.DTOs.SaleDate;
using AdminDashboardApplication.DTOs.Sales;
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

        public SalesController(AppDbContext context, IMapper mapper, IPdfService pdfService)
        {
            _context = context;
            _mapper = mapper;
            _pdfService = pdfService;
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
            Description = "Creates a sale entry in the system and generates a PDF receipt stored in wwwroot/recibos.",
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

            // 1. Save sale in DB (example: your real logic may vary)
            var saleEntity = _mapper.Map<Sales>(dto);
            _context.Sales.Add(saleEntity);
            await _context.SaveChangesAsync();

            // 2. Generate PDF
            string pdfUrl = _pdfService.GenerateReceiptPdf(dto);

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
            Description = "Returns a previously generated receipt located in wwwroot/recibos.",
            OperationId = "DownloadReceipt",
            Tags = new[] { "Sales" }
        )]
        public IActionResult Download([FromQuery] string file)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "recibos", file);

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
