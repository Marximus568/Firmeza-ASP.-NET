
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdminDashboard.Domain.Entities;
using AdminDashboard.Infrastructure.Persistence.Context;
using AdminDashboardApplication.DTOs.Users;

namespace Firmeza.WebApi.Controllers
{
    [ApiController]
    [Route("apiv1/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public UsersController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // =========================================================
        // GET: apiv1/users
        // =========================================================
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var clients = await _context.Users.ToListAsync();
            var usersDto = _mapper.Map<List<UserDto>>(clients);
            return Ok(usersDto);
        }

        // =========================================================
        // GET: apiv1/users/{id}
        // =========================================================
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            var client = await _context.Users.FindAsync(id);

            if (client == null)
                return NotFound(new { message = $"User with ID {id} not found." });

            var userDto = _mapper.Map<UserDto>(client);
            return Ok(userDto);
        }

        // =========================================================
        // PUT: apiv1/users/{id}
        // =========================================================
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, [FromBody] CreateUserDto updateUserDto)
        {
            var client = await _context.Users.FindAsync(id);
            if (client == null)
                return NotFound(new { message = $"User with ID {id} not found." });

            // Map updated fields
            _mapper.Map(updateUserDto, client);

            _context.Entry(client).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // =========================================================
        // POST: apiv1/users
        // =========================================================
        [HttpPost]
        public async Task<ActionResult<UserDto>> PostUser([FromBody] CreateUserDto createUserDto)
        {
            var client = _mapper.Map<Clients>(createUserDto);
            _context.Users.Add(client);
            await _context.SaveChangesAsync();

            var userDto = _mapper.Map<UserDto>(client);
            return CreatedAtAction(nameof(GetUser), new { id = client.Id }, userDto);
        }

        // =========================================================
        // DELETE: apiv1/users/{id}
        // =========================================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var client = await _context.Users.FindAsync(id);
            if (client == null)
                return NotFound(new { message = $"User with ID {id} not found." });

            _context.Users.Remove(client);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // =========================================================
        // Helper: Check existence
        // =========================================================
        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
