using AdminDashboard.Application.Product;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdminDashboard.Domain.Entities;
using AdminDashboard.Infrastructure.Persistence.Context;
using AdminDashboardApplication.DTOs.Products;
using Swashbuckle.AspNetCore.Annotations;

namespace Firmeza.WebApi.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ProductsController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // =========================================================
        // GET: apiv1/products
        // =========================================================
        /// <summary>
        /// Retrieves all products in the system.
        /// </summary>
        /// <returns>A list of product DTOs (no sensitive data).</returns>
        [HttpGet]
        [SwaggerOperation(Summary = "Get all products", Description = "Returns a list of product DTOs without sensitive data.")]
        [ProducesResponseType(typeof(IEnumerable<ProductDto>), 200)]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            var products = await _context.Products.ToListAsync();
            var productsDto = _mapper.Map<List<ProductDto>>(products);
            return Ok(productsDto);
        }

        // =========================================================
        // GET: apiv1/products/{id}
        // =========================================================
        /// <summary>
        /// Retrieves a product by its unique ID.
        /// </summary>
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get product by ID", Description = "Returns a single product DTO if found.")]
        [ProducesResponseType(typeof(ProductDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound(new { message = $"Product with ID {id} not found." });

            var productDto = _mapper.Map<ProductDto>(product);
            return Ok(productDto);
        }

        // =========================================================
        // POST: apiv1/products
        // =========================================================
        /// <summary>
        /// Creates a new product.
        /// </summary>
        [HttpPost]
        [SwaggerOperation(Summary = "Create a product", Description = "Creates a new product and returns its DTO.")]
        [ProducesResponseType(typeof(ProductDto), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ProductDto>> PostProduct([FromBody] CreateProductDto dto)
        {
            var entity = _mapper.Map<Products>(dto);
            _context.Products.Add(entity);
            await _context.SaveChangesAsync();

            var productDto = _mapper.Map<ProductDto>(entity);
            return CreatedAtAction(nameof(GetProduct), new { id = entity.Id }, productDto);
        }

        // =========================================================
        // PUT: apiv1/products/{id}
        // =========================================================
        /// <summary>
        /// Updates an existing product.
        /// </summary>
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Update a product", Description = "Updates a product using its ID.")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> PutProduct(int id, [FromBody] CreateProductDto dto)
        {
            var entity = await _context.Products.FindAsync(id);
            if (entity == null)
                return NotFound(new { message = $"Product with ID {id} not found." });

            _mapper.Map(dto, entity);
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // =========================================================
        // DELETE: apiv1/products/{id}
        // =========================================================
        /// <summary>
        /// Deletes a product by ID.
        /// </summary>
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Delete a product", Description = "Deletes a product by its ID.")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var entity = await _context.Products.FindAsync(id);
            if (entity == null)
                return NotFound(new { message = $"Product with ID {id} not found." });

            _context.Products.Remove(entity);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
