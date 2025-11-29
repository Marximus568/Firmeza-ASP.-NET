using AdminDashboard.Domain.Entities;
using AdminDashboard.Identity.Entities;
using AdminDashboard.Infrastructure.Persistence.Context;
using AdminDashboardApplication.DTOs.Users;
using AdminDashboardApplication.DTOs.Users.Interface;
using AdminDashboardApplication.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DateTime = System.DateTime;
using Enumerable = System.Linq.Enumerable;
using InvalidOperationException = System.InvalidOperationException;

namespace AdminDashboard.Infrastructure.Services.UsersServices
{
    public class UsersService : IUsersService
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly UserManager<ApplicationUserIdentity> _userManager;

        public UsersService(AppDbContext context, IPasswordHasher passwordHasher, UserManager<ApplicationUserIdentity> userManager)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _userManager = userManager;
        }

        // --------------------------------------------------------------------------------------------
        // CREATE
        // --------------------------------------------------------------------------------------------
        public async Task<UserDto> CreateAsync(CreateUserDto createUserDto)
        {
            if (await EmailExistsAsync(createUserDto.Email))
                throw new InvalidOperationException($"Email '{createUserDto.Email}' is already in use.");

            var user = MapToEntity(createUserDto);
            
            // Hash the password before saving
            user.Password = _passwordHasher.HashPassword(createUserDto.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return MapToDto(user);
        }

        // --------------------------------------------------------------------------------------------
        // GET BY ID
        // --------------------------------------------------------------------------------------------
        public async Task<UserDto?> GetByIdAsync(int id)
        {
            var user = await _context.Users
                .Include(u => u.Sales)
                .FirstOrDefaultAsync(u => u.Id == id);

            return user != null ? MapToDto(user) : null;
        }

        // --------------------------------------------------------------------------------------------
        // GET ALL
        // --------------------------------------------------------------------------------------------
        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var users = await _context.Users
                .Include(u => u.Sales)
                .ToListAsync();

            return MapToDtoList(users);
        }

        // --------------------------------------------------------------------------------------------
        // UPDATE
        // --------------------------------------------------------------------------------------------
        public async Task<UserDto?> UpdateAsync(UpdateUserDto updateUserDto)
        {
            var user = await _context.Users
                .Include(u => u.Sales)
                .FirstOrDefaultAsync(u => u.Id == updateUserDto.Id);

            if (user == null)
                return null;

            if (await EmailExistsAsync(updateUserDto.Email, updateUserDto.Id))
                throw new InvalidOperationException(
                    $"Email '{updateUserDto.Email}' is already in use by another user.");

            // Check if role is changing
            var roleChanged = user.Role != updateUserDto.Role;
            var oldRole = user.Role;

            UpdateEntity(updateUserDto, user);
            
            // Hash password if a new one was provided
            if (!string.IsNullOrWhiteSpace(updateUserDto.Password))
            {
                user.Password = _passwordHasher.HashPassword(updateUserDto.Password);
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            // Sync role with Identity if it changed
            if (roleChanged)
            {
                await SyncRoleWithIdentityAsync(user.Email, oldRole, updateUserDto.Role);
            }

            return MapToDto(user);
        }

        // --------------------------------------------------------------------------------------------
        // SYNC ROLE WITH IDENTITY
        // --------------------------------------------------------------------------------------------
        private async Task SyncRoleWithIdentityAsync(string email, string oldRole, string newRole)
        {
            var identityUser = await _userManager.FindByEmailAsync(email);
            
            if (identityUser != null)
            {
                // Remove old role
                if (!string.IsNullOrEmpty(oldRole))
                {
                    var isInOldRole = await _userManager.IsInRoleAsync(identityUser, oldRole);
                    if (isInOldRole)
                    {
                        await _userManager.RemoveFromRoleAsync(identityUser, oldRole);
                    }
                }
                
                // Add new role
                if (!string.IsNullOrEmpty(newRole))
                {
                    var isInNewRole = await _userManager.IsInRoleAsync(identityUser, newRole);
                    if (!isInNewRole)
                    {
                        await _userManager.AddToRoleAsync(identityUser, newRole);
                    }
                }
            }
        }

        // --------------------------------------------------------------------------------------------
        // DELETE
        // --------------------------------------------------------------------------------------------
        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return false;

            // Also delete from Identity database to allow email reuse
            var identityUser = await _userManager.FindByEmailAsync(user.Email);
            if (identityUser != null)
            {
                var identityResult = await _userManager.DeleteAsync(identityUser);
                if (!identityResult.Succeeded)
                {
                    // Log the error but continue with business database deletion
                    // You could throw an exception here if you want strict synchronization
                    var errors = string.Join(", ", identityResult.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Failed to delete user from Identity: {errors}");
                }
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return true;
        }

        // --------------------------------------------------------------------------------------------
        // SEARCH + FILTERING + PAGINATION
        // --------------------------------------------------------------------------------------------
        public async Task<(IEnumerable<UserDto> Users, int TotalCount)> SearchAsync(UserFilterDto filter)
        {
            var query = _context.Users
                .Include(u => u.Sales)
                .AsQueryable();

            // TEXT SEARCH
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var s = filter.SearchTerm.Trim().ToLower();
                query = query.Where(u =>
                    u.FirstName.ToLower().Contains(s) ||
                    u.LastName.ToLower().Contains(s) ||
                    u.Email.ToLower().Contains(s));
            }

            // ROLE FILTER
            if (!string.IsNullOrWhiteSpace(filter.Role))
            {
                var role = filter.Role.Trim();
                query = query.Where(u => u.Role == role);
            }

            // AGE FILTER (CONVERTED TO DATEONLY RANGE)
            if (filter.MinAge.HasValue || filter.MaxAge.HasValue)
            {
                var today = DateOnly.FromDateTime(DateTime.Today);

                if (filter.MinAge.HasValue)
                {
                    var maxDate = today.AddYears(-filter.MinAge.Value);
                    query = query.Where(u => u.DateOfBirth <= maxDate);
                }

                if (filter.MaxAge.HasValue)
                {
                    var minDate = today.AddYears(-(filter.MaxAge.Value + 1)).AddDays(1);
                    query = query.Where(u => u.DateOfBirth >= minDate);
                }
            }

            // EMAIL DOMAIN FILTER
            if (!string.IsNullOrWhiteSpace(filter.EmailDomain))
            {
                var domain = filter.EmailDomain.Trim().ToLower();
                query = query.Where(u =>
                    u.Email.ToLower().EndsWith($"@{domain}") ||
                    u.Email.ToLower().EndsWith(domain));
            }

            // TOTAL BEFORE PAGING
            var totalCount = await query.CountAsync();

            // SORTING
            query = ApplySorting(query, filter.SortBy ?? "", filter.SortDirection ?? "asc");

            // PAGINATION
            var page = filter.PageNumber <= 0 ? 1 : filter.PageNumber;
            var size = filter.PageSize <= 0 ? 10 : Math.Min(filter.PageSize, 100);

            var paged = await query
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync();

            return (MapToDtoList(paged), totalCount);
        }

        // --------------------------------------------------------------------------------------------
        // CHECK EMAIL UNIQUE
        // --------------------------------------------------------------------------------------------
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

        // --------------------------------------------------------------------------------------------
        // GET USERS BY ROLE
        // --------------------------------------------------------------------------------------------
        public async Task<IEnumerable<UserDto>> GetByRoleAsync(string role)
        {
            if (string.IsNullOrWhiteSpace(role))
                return Enumerable.Empty<UserDto>();

            var users = await _context.Users
                .Include(u => u.Sales)
                .Where(u => u.Role == role)
                .ToListAsync();

            return MapToDtoList(users);
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _context.Users.CountAsync();
        }

        // --------------------------------------------------------------------------------------------
        // SORTING
        // --------------------------------------------------------------------------------------------
        private IQueryable<Clients> ApplySorting(IQueryable<Clients> query, string sortBy, string sortDirection)
        {
            var desc = sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase);

            return sortBy.ToLower() switch
            {
                "firstname" => desc ? query.OrderByDescending(u => u.FirstName) : query.OrderBy(u => u.FirstName),
                "lastname" => desc ? query.OrderByDescending(u => u.LastName) : query.OrderBy(u => u.LastName),
                "email" => desc ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email),
                "dateofbirth" => desc ? query.OrderByDescending(u => u.DateOfBirth) : query.OrderBy(u => u.DateOfBirth),
                "role" => desc ? query.OrderByDescending(u => u.Role) : query.OrderBy(u => u.Role),
                _ => query.OrderBy(u => u.FirstName)
            };
        }

        // --------------------------------------------------------------------------------------------
        // ENTITY MAPPERS — WITH DATEONLY CORRECTLY HANDLED
        // --------------------------------------------------------------------------------------------
        private static Clients MapToEntity(CreateUserDto dto)
        {
            return new Clients
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,

                DateOfBirth = dto.DateOfBirth,


                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address,
                Role = dto.Role ?? "Client"
            };
        }

        private static UserDto MapToDto(Clients client)
        {
            if (client == null) return null!;

            int age = 0;
            var today = DateOnly.FromDateTime(DateTime.Today);

            if (client.DateOfBirth != default)
            {
                age = today.Year - client.DateOfBirth.Year;
                if (client.DateOfBirth > today.AddYears(-age))
                    age--;
            }

            return new UserDto
            {
                Id = client.Id,
                FirstName = client.FirstName,
                LastName = client.LastName,
                FullName = $"{client.FirstName} {client.LastName}".Trim(),
                Email = client.Email,
                DateOfBirth = client.DateOfBirth,
                Age = age,
                PhoneNumber = client.PhoneNumber,
                Address = client.Address,
                Role = client.Role,
                Password = client.Password,
                TotalSales = client.Sales?.Count ?? 0
            };
        }

        private static IEnumerable<UserDto> MapToDtoList(IEnumerable<Clients> users)
            => users?.Select(MapToDto).ToList() ?? new List<UserDto>();

        private static void UpdateEntity(UpdateUserDto dto, Clients entity)
        {
            entity.FirstName = dto.FirstName;
            entity.LastName = dto.LastName;
            entity.Email = dto.Email;
            entity.DateOfBirth = dto.DateOfBirth;

            entity.PhoneNumber = dto.PhoneNumber;
            entity.Address = dto.Address;
            entity.Role = dto.Role;
        }
    }
}