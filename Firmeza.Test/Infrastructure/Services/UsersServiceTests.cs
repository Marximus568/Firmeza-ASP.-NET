using AdminDashboard.Domain.Entities;
using AdminDashboard.Identity.Entities;
using AdminDashboard.Infrastructure.Persistence.Context;
using AdminDashboard.Infrastructure.Services.UsersServices;
using AdminDashboardApplication.DTOs.Users;
using AdminDashboardApplication.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Firmeza.Test.Infrastructure.Services;

public class UsersServiceTests
{
    private readonly Mock<IPasswordHasher> _mockPasswordHasher;
    private readonly Mock<UserManager<ApplicationUserIdentity>> _mockUserManager;
    private readonly AppDbContext _context;
    private readonly UsersService _usersService;

    public UsersServiceTests()
    {
        // Setup in-memory database
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _context = new AppDbContext(options);
        _mockPasswordHasher = new Mock<IPasswordHasher>();
        
        // Mock UserManager
        var userStoreMock = new Mock<IUserStore<ApplicationUserIdentity>>();
        _mockUserManager = new Mock<UserManager<ApplicationUserIdentity>>(
            userStoreMock.Object, null, null, null, null, null, null, null, null);
        
        _usersService = new UsersService(_context, _mockPasswordHasher.Object, _mockUserManager.Object);
    }

    [Fact]
    public async Task CreateAsync_WithValidDto_ShouldCreateUserWithHashedPassword()
    {
        // Arrange
        var dto = new CreateUserDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Password = "PlainPassword123",
            PhoneNumber = "1234567890",
            Address = "123 Main St"
        };

        var hashedPassword = "$2a$12$hashedpassword";
        _mockPasswordHasher.Setup(x => x.HashPassword(dto.Password)).Returns(hashedPassword);

        // Act
        var result = await _usersService.CreateAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be(dto.Email);
        result.FirstName.Should().Be(dto.FirstName);
        
        _mockPasswordHasher.Verify(x => x.HashPassword(dto.Password), Times.Once);
        
        var userInDb = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        userInDb.Should().NotBeNull();
        userInDb!.Password.Should().Be(hashedPassword);
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateEmail_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var existingUser = new Clients("Jane", "Doe", "jane@example.com", "1234567890");
        _context.Users.Add(existingUser);
        await _context.SaveChangesAsync();

        var dto = new CreateUserDto
        {
            Email = "jane@example.com",
            FirstName = "Jane",
            LastName = "Smith",
            Password = "password"
        };

        // Act
        Func<Task> act = async () => await _usersService.CreateAsync(dto);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*already in use*");
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ShouldReturnUser()
    {
        // Arrange
        var client = new Clients("John", "Doe", "john@example.com", "1234567890");
        _context.Users.Add(client);
        await _context.SaveChangesAsync();

        // Act
        var result = await _usersService.GetByIdAsync(client.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(client.Id);
        result.Email.Should().Be(client.Email);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistentId_ShouldReturnNull()
    {
        // Arrange
        var nonExistentId = 9999;

        // Act
        var result = await _usersService.GetByIdAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllUsers()
    {
        // Arrange
        var users = new List<Clients>
        {
            new Clients("John", "Doe", "john@example.com", "1234567890"),
            new Clients("Jane", "Smith", "jane@example.com", "0987654321"),
            new Clients("Bob", "Johnson", "bob@example.com", "5555555555")
        };
        
        _context.Users.AddRange(users);
        await _context.SaveChangesAsync();

        // Act
        var result = await _usersService.GetAllAsync();

        // Assert
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task UpdateAsync_WithValidDto_ShouldUpdateUser()
    {
        // Arrange
        var client = new Clients("John", "Doe", "john@example.com", "1234567890");
        _context.Users.Add(client);
        await _context.SaveChangesAsync();

        var updateDto = new UpdateUserDto
        {
            Id = client.Id,
            FirstName = "Johnny",
            LastName = "Doe",
            Email = "johnny@example.com",
            PhoneNumber = "9999999999",
            Address = "456 New St",
            Role = "Admin"
        };

        // Act
        var result = await _usersService.UpdateAsync(updateDto);

        // Assert
        result.Should().NotBeNull();
        result!.FirstName.Should().Be("Johnny");
        result.Email.Should().Be("johnny@example.com");
        result.Role.Should().Be("Admin");
    }

    [Fact]
    public async Task UpdateAsync_WithNewPassword_ShouldHashAndUpdatePassword()
    {
        // Arrange
        var client = new Clients("John", "Doe", "john@example.com", "1234567890")
        {
            Password = "oldhashedpassword"
        };
        _context.Users.Add(client);
        await _context.SaveChangesAsync();

        var newHashedPassword = "$2a$12$newhashedpassword";
        _mockPasswordHasher.Setup(x => x.HashPassword("NewPassword123")).Returns(newHashedPassword);

        var updateDto = new UpdateUserDto
        {
            Id = client.Id,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            Password = "NewPassword123"
        };

        // Act
        var result = await _usersService.UpdateAsync(updateDto);

        // Assert
        _mockPasswordHasher.Verify(x => x.HashPassword("NewPassword123"), Times.Once);
        
        var updatedUser = await _context.Users.FindAsync(client.Id);
        updatedUser!.Password.Should().Be(newHashedPassword);
    }

    [Fact]
    public async Task UpdateAsync_WithNullPassword_ShouldNotUpdatePassword()
    {
        // Arrange
        var originalPassword = "originalhashedpassword";
        var client = new Clients("John", "Doe", "john@example.com", "1234567890")
        {
            Password = originalPassword
        };
        _context.Users.Add(client);
        await _context.SaveChangesAsync();

        var updateDto = new UpdateUserDto
        {
            Id = client.Id,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            Password = null
        };

        // Act
        await _usersService.UpdateAsync(updateDto);

        // Assert
        _mockPasswordHasher.Verify(x => x.HashPassword(It.IsAny<string>()), Times.Never);
        
        var updatedUser = await _context.Users.FindAsync(client.Id);
        updatedUser!.Password.Should().Be(originalPassword);
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistentId_ShouldReturnNull()
    {
        // Arrange
        var updateDto = new UpdateUserDto
        {
            Id = 9999,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com"
        };

        // Act
        var result = await _usersService.UpdateAsync(updateDto);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_WithDuplicateEmail_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var user1 = new Clients("John", "Doe", "john@example.com", "1234567890");
        var user2 = new Clients("Jane", "Smith", "jane@example.com", "0987654321");
        _context.Users.AddRange(user1, user2);
        await _context.SaveChangesAsync();

        var updateDto = new UpdateUserDto
        {
            Id = user1.Id,
            FirstName = "John",
            LastName = "Doe",
            Email = "jane@example.com" // Trying to use user2's email
        };

        // Act
        Func<Task> act = async () => await _usersService.UpdateAsync(updateDto);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*already in use*");
    }

    [Fact]
    public async Task DeleteAsync_WithExistingId_ShouldReturnTrueAndDeleteUser()
    {
        // Arrange
        var client = new Clients("John", "Doe", "john@example.com", "1234567890");
        _context.Users.Add(client);
        await _context.SaveChangesAsync();
        var userId = client.Id;

        // Mock Identity user deletion
        var identityUser = new ApplicationUserIdentity { Email = "john@example.com" };
        _mockUserManager.Setup(x => x.FindByEmailAsync("john@example.com"))
            .ReturnsAsync(identityUser);
        _mockUserManager.Setup(x => x.DeleteAsync(identityUser))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _usersService.DeleteAsync(userId);

        // Assert
        result.Should().BeTrue();
        
        // Verify Identity deletion was called
        _mockUserManager.Verify(x => x.FindByEmailAsync("john@example.com"), Times.Once);
        _mockUserManager.Verify(x => x.DeleteAsync(identityUser), Times.Once);
        
        var deletedUser = await _context.Users.FindAsync(userId);
        deletedUser.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistentId_ShouldReturnFalse()
    {
        // Arrange
        var nonExistentId = 9999;

        // Act
        var result = await _usersService.DeleteAsync(nonExistentId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task EmailExistsAsync_WithExistingEmail_ShouldReturnTrue()
    {
        // Arrange
        var client = new Clients("John", "Doe", "john@example.com", "1234567890");
        _context.Users.Add(client);
        await _context.SaveChangesAsync();

        // Act
        var result = await _usersService.EmailExistsAsync("john@example.com");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task EmailExistsAsync_WithNonExistentEmail_ShouldReturnFalse()
    {
        // Arrange & Act
        var result = await _usersService.EmailExistsAsync("nonexistent@example.com");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task EmailExistsAsync_IsCaseInsensitive()
    {
        // Arrange
        var client = new Clients("John", "Doe", "John@Example.com", "1234567890");
        _context.Users.Add(client);
        await _context.SaveChangesAsync();

        // Act
        var result = await _usersService.EmailExistsAsync("john@example.com");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task EmailExistsAsync_WithExcludeUserId_ShouldExcludeThatUser()
    {
        // Arrange
        var client = new Clients("John", "Doe", "john@example.com", "1234567890");
        _context.Users.Add(client);
        await _context.SaveChangesAsync();

        // Act
        var result = await _usersService.EmailExistsAsync("john@example.com", client.Id);

        // Assert
        result.Should().BeFalse(); // Should exclude this user
    }

    [Fact]
    public async Task SearchAsync_WithSearchTerm_ShouldFilterResults()
    {
        // Arrange
        var users = new List<Clients>
        {
            new Clients("John", "Doe", "john@example.com", "1234567890"),
            new Clients("Jane", "Smith", "jane@example.com", "0987654321"),
            new Clients("Bob", "Johnson", "bob@example.com", "5555555555")
        };
        _context.Users.AddRange(users);
        await _context.SaveChangesAsync();

        var filter = new UserFilterDto { SearchTerm = "john" };

        // Act
        var (results, totalCount) = await _usersService.SearchAsync(filter);

        // Assert
        results.Should().HaveCount(2); // John Doe and Bob Johnson
        totalCount.Should().Be(2);
    }

    [Fact]
    public async Task SearchAsync_WithRoleFilter_ShouldFilterByRole()
    {
        // Arrange
        var users = new List<Clients>
        {
            new Clients { FirstName = "Admin1", LastName = "User", Email = "admin1@example.com", Role = "Admin" },
            new Clients { FirstName = "Client1", LastName = "User", Email = "client1@example.com", Role = "Client" },
            new Clients { FirstName = "Admin2", LastName = "User", Email = "admin2@example.com", Role = "Admin" }
        };
        _context.Users.AddRange(users);
        await _context.SaveChangesAsync();

        var filter = new UserFilterDto { Role = "Admin" };

        // Act
        var (results, totalCount) = await _usersService.SearchAsync(filter);

        // Assert
        results.Should().HaveCount(2);
        totalCount.Should().Be(2);
        results.Should().OnlyContain(u => u.Role == "Admin");
    }

    [Fact]
    public async Task SearchAsync_WithPagination_ShouldReturnCorrectPage()
    {
        // Arrange
        var users = Enumerable.Range(1, 25)
            .Select(i => new Clients($"User{i}", "Test", $"user{i}@example.com", "1234567890"))
            .ToList();
        _context.Users.AddRange(users);
        await _context.SaveChangesAsync();

        var filter = new UserFilterDto { PageNumber = 2, PageSize = 10 };

        // Act
        var (results, totalCount) = await _usersService.SearchAsync(filter);

        // Assert
        results.Should().HaveCount(10); // Page 2 should have 10 users
        totalCount.Should().Be(25);
    }

    [Fact]
    public async Task GetByRoleAsync_ShouldReturnUsersWithSpecificRole()
    {
        // Arrange
        var users = new List<Clients>
        {
            new Clients { FirstName = "Admin1", Email = "admin1@example.com", Role = "Admin" },
            new Clients { FirstName = "Client1", Email = "client1@example.com", Role = "Client" },
            new Clients { FirstName = "Admin2", Email = "admin2@example.com", Role = "Admin" }
        };
        _context.Users.AddRange(users);
        await _context.SaveChangesAsync();

        // Act
        var result = await _usersService.GetByRoleAsync("Admin");

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(u => u.Role == "Admin");
    }

    [Fact]
    public async Task GetTotalCountAsync_ShouldReturnCorrectCount()
    {
        // Arrange
        var users = Enumerable.Range(1, 15)
            .Select(i => new Clients($"User{i}", "Test", $"user{i}@example.com", "1234567890"))
            .ToList();
        _context.Users.AddRange(users);
        await _context.SaveChangesAsync();

        // Act
        var count = await _usersService.GetTotalCountAsync();

        // Assert
        count.Should().Be(15);
    }
}
