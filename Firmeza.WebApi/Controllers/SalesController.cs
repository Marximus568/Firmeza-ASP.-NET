using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdminDashboard.Domain.Entities;
using AdminDashboard.Infrastructure.Persistence.Context;
using AdminDashboardApplication.DTOs.Sales;
using Swashbuckle.AspNetCore.Annotations;

namespace Firmeza.WebApi.Controllers
{
    /// <summary>
    /// API controller for managing sales records. Provides CRUD operations for sales data.
    /// </summary>
    /// <remarks>
    /// This controller handles HTTP requests for sales management in the Admin Dashboard application.
    /// All endpoints return standard HTTP status codes and JSON responses.
    /// </remarks>
    [ApiController]
    [Route("apiv1/[controller]")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class SalesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="SalesController"/> class.
        /// </summary>
        /// <param name="context">The database context for accessing sales data.</param>
        /// <param name="mapper">The AutoMapper instance for object-to-DTO mapping.</param>
        public SalesController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // =========================================================
        // GET: apiv1/sales
        // =========================================================
        /// <summary>
        /// Retrieves a list of all sales records.
        /// </summary>
        /// <returns>
        /// An <see cref="ActionResult"/> containing a collection of <see cref="SaleDto"/> objects.
        /// </returns>
        /// <response code="200">Returns the list of sales successfully.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [HttpGet]
        [SwaggerOperation(
            Summary = "Get all sales",
            Description = "Retrieves all sales records from the database and maps them to DTOs.",
            OperationId = "GetAllSales",
            Tags = new[] { "Sales" }
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(IEnumerable<SaleDto>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
        public async Task<ActionResult<IEnumerable<SaleDto>>> GetSales()
        {
            var sales = await _context.Sales.ToListAsync();
            var salesDto = _mapper.Map<List<SaleDto>>(sales);
            return Ok(salesDto);
        }

        // =========================================================
        // GET: apiv1/sales/{id}
        // =========================================================
        /// <summary>
        /// Retrieves a specific sale by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the sale to retrieve.</param>
        /// <returns>
        /// An <see cref="ActionResult"/> containing the <see cref="SaleDto"/> if found.
        /// </returns>
        /// <response code="200">Sale found and returned successfully.</response>
        /// <response code="404">If no sale exists with the specified ID.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Get sale by ID",
            Description = "Fetches a single sale record by its primary key and returns it as a DTO.",
            OperationId = "GetSaleById",
            Tags = new[] { "Sales" }
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Sale retrieved successfully", typeof(SaleDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Sale not found", typeof(object))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
        public async Task<ActionResult<SaleDto>> GetSale(int id)
        {
            var sale = await _context.Sales.FindAsync(id);
            if (sale == null)
                return NotFound(new { message = $"Sale with ID {id} not found." });

            var saleDto = _mapper.Map<SaleDto>(sale);
            return Ok(saleDto);
        }

        // =========================================================
        // POST: apiv1/sales
        // =========================================================
        /// <summary>
        /// Creates a new sale record.
        /// </summary>
        /// <param name="dto">The sale data transfer object containing the sale information to create.</param>
        /// <returns>
        /// An <see cref="ActionResult"/> containing the created <see cref="SaleDto"/> with its assigned ID.
        /// </returns>
        /// <response code="201">Sale created successfully.</response>
        /// <response code="400">If the request payload is invalid or malformed.</response>
        /// <response code="500">If an internal server error occurs during creation.</response>
        [HttpPost]
        [SwaggerOperation(
            Summary = "Create a new sale",
            Description = "Maps the incoming DTO to a domain entity, persists it, and returns the created resource.",
            OperationId = "CreateSale",
            Tags = new[] { "Sales" }
        )]
        [SwaggerResponse(StatusCodes.Status201Created, "Sale created", typeof(SaleDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request data")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
        public async Task<ActionResult<SaleDto>> PostSale([FromBody] CreateSaleDto dto)
        {
            var entity = _mapper.Map<Sales>(dto);
            _context.Sales.Add(entity);
            await _context.SaveChangesAsync();

            var result = _mapper.Map<SaleDto>(entity);
            return CreatedAtAction(nameof(GetSale), new { id = entity.Id }, result);
        }

        // =========================================================
        // PUT: apiv1/sales/{id}
        // =========================================================
        /// <summary>
        /// Updates an existing sale record.
        /// </summary>
        /// <param name="id">The ID of the sale to update.</param>
        /// <param name="dto">The updated sale data in DTO format.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the update operation.
        /// </returns>
        /// <response code="204">Sale updated successfully.</response>
        /// <response code="404">If no sale exists with the specified ID.</response>
        /// <response code="400">If the ID in the route does not match the ID in the payload (if applicable).</response>
        /// <response code="500">If an internal server error occurs.</response>
        [HttpPut("{id}")]
        [SwaggerOperation(
            Summary = "Update an existing sale",
            Description = "Finds the sale by ID, maps updated values from DTO, and persists changes.",
            OperationId = "UpdateSale",
            Tags = new[] { "Sales" }
        )]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Sale updated successfully")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Sale not found", typeof(object))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request or mismatched ID")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
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

        // =========================================================
        // DELETE: apiv1/sales/{id}
        // =========================================================
        /// <summary>
        /// Deletes a sale record by its ID.
        /// </summary>
        /// <param name="id">The ID of the sale to delete.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the deletion.
        /// </returns>
        /// <response code="204">Sale deleted successfully.</response>
        /// <response code="404">If no sale exists with the specified ID.</response>
        /// <response code="500">If an internal server error occurs during deletion.</response>
        [HttpDelete("{id}")]
        [SwaggerOperation(
            Summary = "Delete a sale",
            Description = "Removes a sale record permanently from the database.",
            OperationId = "DeleteSale",
            Tags = new[] { "Sales" }
        )]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Sale deleted successfully")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Sale not found", typeof(object))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
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