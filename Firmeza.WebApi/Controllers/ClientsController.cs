using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations; 
using AdminDashboard.Domain.Entities;
using AdminDashboard.Infrastructure.Persistence.Context;
using AdminDashboardApplication.DTOs.Users;

namespace Firmeza.WebApi.Controllers
{
    [ApiController]
    [Route("apiv1/[controller]")]
    [Produces("application/json")]
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
        /// <summary>
        /// Retrieves all registered users.
        /// </summary>
        /// <remarks>
        /// This endpoint returns a list of users mapped to <see cref="UserDto"/>.
        /// Sensitive data such as passwords are not included.
        /// </remarks>
        [HttpGet]
        [SwaggerOperation(Summary = "Get all users", Description = "Returns a list of users without exposing sensitive data.")]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), 200)]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            var usersDto = _mapper.Map<List<UserDto>>(users);
            return Ok(usersDto);
        }

        // =========================================================
        // GET: apiv1/users/{id}
        // =========================================================
        /// <summary>
        /// Retrieves a specific user by ID.
        /// </summary>
        /// <param name="id">User ID to search for.</param>
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get user by ID", Description = "Returns a specific user based on the provided ID.")]
        [ProducesResponseType(typeof(UserDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound(new { message = $"User with ID {id} not found." });

            var userDto = _mapper.Map<UserDto>(user);
            return Ok(userDto);
        }

        // =========================================================
        // POST: apiv1/users
        // =========================================================
        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="createUserDto">The user data for creation.</param>
        [HttpPost]
        [SwaggerOperation(Summary = "Create a new user", Description = "Registers a new user and returns its details as a DTO.")]
        [ProducesResponseType(typeof(UserDto), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<UserDto>> PostUser([FromBody] CreateUserDto createUserDto)
        {
            var entity = _mapper.Map<Clients>(createUserDto);
            _context.Users.Add(entity);
            await _context.SaveChangesAsync();

            var userDto = _mapper.Map<UserDto>(entity);
            return CreatedAtAction(nameof(GetUser), new { id = entity.Id }, userDto);
        }

        // =========================================================
        // PUT: apiv1/users/{id}
        // =========================================================
        /// <summary>
        /// Updates an existing user.
        /// </summary>
        /// <param name="id">ID of the user to update.</param>
        /// <param name="updateUserDto">The updated user data.</param>
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Update an existing user", Description = "Updates a user based on the given ID and DTO.")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> PutUser(int id, [FromBody] CreateUserDto updateUserDto)
        {
            var entity = await _context.Users.FindAsync(id);
            if (entity == null)
                return NotFound(new { message = $"User with ID {id} not found." });

            _mapper.Map(updateUserDto, entity);
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // =========================================================
        // DELETE: apiv1/users/{id}
        // =========================================================
        /// <summary>
        /// Deletes a user by ID.
        /// </summary>
        /// <param name="id">The ID of the user to delete.</param>
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Delete user by ID", Description = "Removes a user record from the database.")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var entity = await _context.Users.FindAsync(id);
            if (entity == null)
                return NotFound(new { message = $"User with ID {id} not found." });

            _context.Users.Remove(entity);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
