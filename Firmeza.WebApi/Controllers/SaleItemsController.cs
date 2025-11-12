using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdminDashboard.Domain.Entities;
using AdminDashboard.Infrastructure.Persistence.Context;

namespace Firmeza.WebApi.Controllers
{
    [Route("apiv1/[controller]")]
    [ApiController]
    public class SaleItemsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SaleItemsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/SaleItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SaleItems>>> GetSaleItems()
        {
            return await _context.SaleItems.ToListAsync();
        }

        // GET: api/SaleItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SaleItems>> GetSaleItems(int id)
        {
            var saleItems = await _context.SaleItems.FindAsync(id);

            if (saleItems == null)
            {
                return NotFound();
            }

            return saleItems;
        }

        // PUT: api/SaleItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSaleItems(int id, SaleItems saleItems)
        {
            if (id != saleItems.Id)
            {
                return BadRequest();
            }

            _context.Entry(saleItems).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SaleItemsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/SaleItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SaleItems>> PostSaleItems(SaleItems saleItems)
        {
            _context.SaleItems.Add(saleItems);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSaleItems", new { id = saleItems.Id }, saleItems);
        }

        // DELETE: api/SaleItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSaleItems(int id)
        {
            var saleItems = await _context.SaleItems.FindAsync(id);
            if (saleItems == null)
            {
                return NotFound();
            }

            _context.SaleItems.Remove(saleItems);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SaleItemsExists(int id)
        {
            return _context.SaleItems.Any(e => e.Id == id);
        }
    }
}
