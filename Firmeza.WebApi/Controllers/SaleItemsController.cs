using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdminDashboard.Domain.Entities;
using AdminDashboard.Infrastructure.Persistence.Context;
using AdminDashboardApplication.DTOs.SaleItems;
using Swashbuckle.AspNetCore.Annotations;

using Microsoft.AspNetCore.Authorization;

namespace Firmeza.WebApi.Controllers
{
    /// <summary>
    /// Handles CRUD operations for sale items.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("v1/[controller]")]
    public class SaleItemsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public SaleItemsController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // =========================================================
        // GET: apiv1/saleitems
        // =========================================================
        /// <summary>
        /// Retrieves all sale items registered in the database.
        /// </summary>
        /// <remarks>
        /// This endpoint returns a list of sale items in DTO format, ensuring no sensitive data is exposed.
        /// </remarks>
        /// <response code="200">Returns the list of sale items.</response>
        [HttpGet]
        [SwaggerOperation(Summary = "Get all sale items", Description = "Fetches all sale items stored in the database.")]
        [ProducesResponseType(typeof(IEnumerable<SaleItemDto>), 200)]
        public async Task<ActionResult<IEnumerable<SaleItemDto>>> GetSaleItems()
        {
            var items = await _context.SaleItems.ToListAsync();
            var dto = _mapper.Map<List<SaleItemDto>>(items);
            return Ok(dto);
        }

        // =========================================================
        // GET: apiv1/saleitems/{id}
        // =========================================================
        /// <summary>
        /// Retrieves a single sale item by its unique ID.
        /// </summary>
        /// <param name="id">The ID of the sale item.</param>
        /// <returns>The sale item details in DTO format.</returns>
        /// <response code="200">Returns the sale item if found.</response>
        /// <response code="404">If the sale item does not exist.</response>
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get sale item by ID", Description = "Fetches a single sale item by its unique identifier.")]
        [ProducesResponseType(typeof(SaleItemDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<SaleItemDto>> GetSaleItem(int id)
        {
            var item = await _context.SaleItems.FindAsync(id);
            if (item == null)
                return NotFound(new { message = $"Sale item with ID {id} not found." });

            var dto = _mapper.Map<SaleItemDto>(item);
            return Ok(dto);
        }

        // =========================================================
        // POST: apiv1/saleitems
        // =========================================================
        /// <summary>
        /// Creates a new sale item record.
        /// </summary>
        /// <param name="dto">The sale item creation DTO.</param>
        /// <returns>The newly created sale item in DTO format.</returns>
        /// <response code="201">Sale item created successfully.</response>
        /// <response code="400">If the provided data is invalid.</response>
        [HttpPost]
        [SwaggerOperation(Summary = "Create sale item", Description = "Adds a new sale item to the database.")]
        [ProducesResponseType(typeof(SaleItemDto), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<SaleItemDto>> PostSaleItem([FromBody] CreateSaleItemDto dto)
        {
            var entity = _mapper.Map<SaleItems>(dto);
            _context.SaleItems.Add(entity);
            await _context.SaveChangesAsync();

            var result = _mapper.Map<SaleItemDto>(entity);
            return CreatedAtAction(nameof(GetSaleItem), new { id = entity.Id }, result);
        }

        // =========================================================
        // PUT: apiv1/saleitems/{id}
        // =========================================================
        /// <summary>
        /// Updates an existing sale item.
        /// </summary>
        /// <param name="id">The ID of the sale item to update.</param>
        /// <param name="dto">The sale item data to update.</param>
        /// <response code="204">Update successful.</response>
        /// <response code="404">If the sale item was not found.</response>
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Update sale item", Description = "Updates the data of an existing sale item.")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> PutSaleItem(int id, [FromBody] CreateSaleItemDto dto)
        {
            var entity = await _context.SaleItems.FindAsync(id);
            if (entity == null)
                return NotFound(new { message = $"Sale item with ID {id} not found." });

            _mapper.Map(dto, entity);
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // =========================================================
        // DELETE: apiv1/saleitems/{id}
        // =========================================================
        /// <summary>
        /// Deletes a sale item by its ID.
        /// </summary>
        /// <param name="id">The ID of the sale item to delete.</param>
        /// <response code="204">Sale item deleted successfully.</response>
        /// <response code="404">If the sale item was not found.</response>
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Delete sale item", Description = "Removes a sale item by its ID.")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteSaleItem(int id)
        {
            var entity = await _context.SaleItems.FindAsync(id);
            if (entity == null)
                return NotFound(new { message = $"Sale item with ID {id} not found." });

            _context.SaleItems.Remove(entity);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
