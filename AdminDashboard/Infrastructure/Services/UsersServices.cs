using AdminDashboard.Application.DTOs.Auth.Mappers;
using AdminDashboard.Application.DTOs.User;
using AdminDashboard.Application.Users.DTOs;
using AdminDashboard.Application.DTOs.User.Interfaces;
using AdminDashboard.Domain.Entities;
using AdminDashboard.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using UserMapper = AdminDashboard.Application.DTOs.Users.Mappers.UserMapper;

namespace AdminDashboard.Infrastructure.Services
{
    /// <summary>
    /// Service implementation for managing user operations.
    /// Handles CRUD, search, filtering, and validation logic.
    /// Implements IUsersService contract.
    /// </summary>
    public class UsersService : IUsersService
    {
        private readonly AppDbContext _context;

        public UsersService(AppDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<UserDto> CreateAsync(CreateUserDto createUserDto)
        {
            // Validate email uniqueness
            if (await EmailExistsAsync(createUserDto.Email))
                throw new InvalidOperationException($"Email '{createUserDto.Email}' is already in use.");

            var user = Application.DTOs.Users.Mappers.UserMapper.ToEntity(createUserDto);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Application.DTOs.Users.Mappers.UserMapper.ToDto(user);
        }

        /// <inheritdoc />
        public async Task<UserDto?> GetByIdAsync(int id)
        {
            var user = await _context.Users
                .Include(u => u.Sales)
                .FirstOrDefaultAsync(u => u.Id == id);

            return user != null ? Application.DTOs.Users.Mappers.UserMapper.ToDto(user) : null;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var users = await _context.Users
                .Include(u => u.Sales)
                .ToListAsync();

            return Application.DTOs.Users.Mappers.UserMapper.ToDtoList(users);
        }

        /// <inheritdoc />
        public async Task<UserDto?> UpdateAsync(UpdateUserDto updateUserDto)
        {
            var user = await _context.Users
                .Include(u => u.Sales)
                .FirstOrDefaultAsync(u => u.Id == updateUserDto.Id);

            if (user == null)
                return null;

            // Verify email uniqueness (exclude current user)
            if (await EmailExistsAsync(updateUserDto.Email, updateUserDto.Id))
                throw new InvalidOperationException($"Email '{updateUserDto.Email}' is already in use by another user.");

            // Map update DTO onto entity
            Application.DTOs.Users.Mappers.UserMapper.UpdateEntity(updateUserDto, user);

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Application.DTOs.Users.Mappers.UserMapper.ToDto(user);
        }

        /// <inheritdoc />
        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return true;
        }

        /// <inheritdoc />
        public async Task<(IEnumerable<UserDto> Users, int TotalCount)> SearchAsync(UserFilterDto filter)
        {
            // Start query
            var query = _context.Users
                .Include(u => u.Sales)
                .AsQueryable();

            // Search term: first name, last name, email
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var s = filter.SearchTerm.Trim().ToLower();
                query = query.Where(u =>
                    u.FirstName.ToLower().Contains(s) ||
                    u.LastName.ToLower().Contains(s) ||
                    u.Email.ToLower().Contains(s));
            }

            // Role filter
            if (!string.IsNullOrWhiteSpace(filter.Role))
            {
                var role = filter.Role.Trim();
                query = query.Where(u => u.Role == role);
            }

            // Age filters (convert ages to date range)
            if (filter.MinAge.HasValue || filter.MaxAge.HasValue)
            {
                var today = DateTime.Today;

                if (filter.MinAge.HasValue)
                {
                    // users born on or before maxDate are at least MinAge
                    var maxDate = today.AddYears(-filter.MinAge.Value);
                    query = query.Where(u => u.DateOfBirth <= maxDate);
                }

                if (filter.MaxAge.HasValue)
                {
                    // users born on or after minDate are at most MaxAge
                    var minDate = today.AddYears(-filter.MaxAge.Value - 1).AddDays(1);
                    query = query.Where(u => u.DateOfBirth >= minDate);
                }
            }

            // Email domain filter
            if (!string.IsNullOrWhiteSpace(filter.EmailDomain))
            {
                var domain = filter.EmailDomain.Trim().ToLower();
                query = query.Where(u => u.Email.ToLower().EndsWith($"@{domain}") || u.Email.ToLower().EndsWith(domain));
            }

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Sorting
            query = ApplySorting(query, filter.SortBy ?? string.Empty, filter.SortDirection ?? "asc");

            // Pagination defaults
            var pageNumber = filter.PageNumber <= 0 ? 1 : filter.PageNumber;
            var pageSize = filter.PageSize <= 0 ? 10 : Math.Min(filter.PageSize, 100);

            var paged = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var dtos = Application.DTOs.Users.Mappers.UserMapper.ToDtoList(paged);

            return (dtos, totalCount);
        }

        /// <inheritdoc />
        public async Task<bool> EmailExistsAsync(string email, int? excludeUserId = null)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            var normalized = email.Trim().ToLower();
            var query = _context.Users.Where(u => u.Email.ToLower() == normalized);

            if (excludeUserId.HasValue)
                query = query.Where(u => u.Id != excludeUserId.Value);

            return await query.AnyAsync();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<UserDto>> GetByRoleAsync(string role)
        {
            if (string.IsNullOrWhiteSpace(role))
                return Enumerable.Empty<UserDto>();

            var users = await _context.Users
                .Include(u => u.Sales)
                .Where(u => u.Role == role)
                .ToListAsync();

            return UserMapper.ToDtoList(users);
        }

        /// <inheritdoc />
        public async Task<int> GetTotalCountAsync()
        {
            return await _context.Users.CountAsync();
        }

        /// <summary>
        /// Apply sorting to Users query based on field and direction.
        /// </summary>
        private IQueryable<Users> ApplySorting(IQueryable<Users> query, string sortBy, string sortDirection)
        {
            var descending = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);

            return (sortBy ?? string.Empty).ToLower() switch
            {
                "firstname" => descending ? query.OrderByDescending(u => u.FirstName) : query.OrderBy(u => u.FirstName),
                "lastname" => descending ? query.OrderByDescending(u => u.LastName) : query.OrderBy(u => u.LastName),
                "email" => descending ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email),
                "dateofbirth" => descending ? query.OrderByDescending(u => u.DateOfBirth) : query.OrderBy(u => u.DateOfBirth),
                "role" => descending ? query.OrderByDescending(u => u.Role) : query.OrderBy(u => u.Role),
                _ => query.OrderBy(u => u.FirstName)
            };
        }
    }
}
