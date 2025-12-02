using AdminDashboard.Domain.Entities;
using AdminDashboardApplication.DTOs.Users;
using AdminDashboardApplication.DTOs.Users.Mappers;
using FluentAssertions;
using Xunit;

namespace Firmeza.Test.Application.Mappers;

public class UserMapperTests
{
    [Fact]
    public void ToEntity_WithValidCreateUserDto_ShouldMapCorrectly()
    {
        // Arrange
        var dto = new CreateUserDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Password = "SecurePassword123",
            DateOfBirth = new DateOnly(1990, 5, 15),
            PhoneNumber = "1234567890",
            Address = "123 Main St",
            Role = "Admin"
        };

        // Act
        var entity = UserMapper.ToEntity(dto);

        // Assert
        entity.Should().NotBeNull();
        entity.FirstName.Should().Be(dto.FirstName);
        entity.LastName.Should().Be(dto.LastName);
        entity.Email.Should().Be(dto.Email);
        entity.Password.Should().Be(dto.Password);
        entity.DateOfBirth.Should().Be(dto.DateOfBirth);
        entity.PhoneNumber.Should().Be(dto.PhoneNumber);
        entity.Address.Should().Be(dto.Address);
        entity.Role.Should().Be(dto.Role);
    }

    [Fact]
    public void ToEntity_WithNullRole_ShouldDefaultToClient()
    {
        // Arrange
        var dto = new CreateUserDto
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane@example.com",
            Password = "password",
            Role = null
        };

        // Act
        var entity = UserMapper.ToEntity(dto);

        // Assert
        entity.Role.Should().Be("Client");
    }

    [Fact]
    public void ToEntity_WithNullDto_ShouldReturnNull()
    {
        // Arrange
        CreateUserDto? dto = null;

        // Act
        var entity = UserMapper.ToEntity(dto!);

        // Assert
        entity.Should().BeNull();
    }

    [Fact]
    public void ToDto_WithValidClient_ShouldMapCorrectly()
    {
        // Arrange
        var client = new Clients
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            DateOfBirth = new DateOnly(1990, 5, 15),
            PhoneNumber = "1234567890",
            Address = "123 Main St",
            Role = "Client",
            Sales = new List<Sales>
            {
                new Sales { Id = 1 },
                new Sales { Id = 2 }
            }
        };

        // Act
        var dto = UserMapper.ToDto(client);

        // Assert
        dto.Should().NotBeNull();
        dto.Id.Should().Be(client.Id);
        dto.FirstName.Should().Be(client.FirstName);
        dto.LastName.Should().Be(client.LastName);
        dto.FullName.Should().Be("John Doe");
        dto.Email.Should().Be(client.Email);
        dto.DateOfBirth.Should().Be(client.DateOfBirth);
        dto.PhoneNumber.Should().Be(client.PhoneNumber);
        dto.Address.Should().Be(client.Address);
        dto.Role.Should().Be(client.Role);
        dto.TotalSales.Should().Be(2);
    }

    [Fact]
    public void ToDto_ShouldCalculateAgeCorrectly()
    {
        // Arrange
        var today = DateTime.Today;
        var birthDate = new DateOnly(today.Year - 30, today.Month, today.Day);
        
        var client = new Clients
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            DateOfBirth = birthDate
        };

        // Act
        var dto = UserMapper.ToDto(client);

        // Assert
        dto.Age.Should().Be(30);
    }

   
    [Fact]
    public void ToDto_WithDefaultDateOfBirth_ShouldHaveZeroAge()
    {
        // Arrange
        var client = new Clients
        {
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com",
            DateOfBirth = default
        };

        // Act
        var dto = UserMapper.ToDto(client);

        // Assert
        dto.Age.Should().Be(0);
    }

    [Fact]
    public void ToDto_WithNullSales_ShouldHaveZeroTotalSales()
    {
        // Arrange
        var client = new Clients
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            Sales = null!
        };

        // Act
        var dto = UserMapper.ToDto(client);

        // Assert
        dto.TotalSales.Should().Be(0);
    }

    [Fact]
    public void ToDto_WithNullClient_ShouldReturnNull()
    {
        // Arrange
        Clients? client = null;

        // Act
        var dto = UserMapper.ToDto(client!);

        // Assert
        dto.Should().BeNull();
    }

    [Fact]
    public void ToDtoList_WithMultipleClients_ShouldMapAll()
    {
        // Arrange
        var clients = new List<Clients>
        {
            new Clients { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com" },
            new Clients { Id = 2, FirstName = "Jane", LastName = "Smith", Email = "jane@example.com" },
            new Clients { Id = 3, FirstName = "Bob", LastName = "Johnson", Email = "bob@example.com" }
        };

        // Act
        var dtos = UserMapper.ToDtoList(clients);

        // Assert
        dtos.Should().HaveCount(3);
        dtos.Should().Contain(d => d.Id == 1 && d.FirstName == "John");
        dtos.Should().Contain(d => d.Id == 2 && d.FirstName == "Jane");
        dtos.Should().Contain(d => d.Id == 3 && d.FirstName == "Bob");
    }

    [Fact]
    public void ToDtoList_WithEmptyList_ShouldReturnEmptyList()
    {
        // Arrange
        var clients = new List<Clients>();

        // Act
        var dtos = UserMapper.ToDtoList(clients);

        // Assert
        dtos.Should().NotBeNull();
        dtos.Should().BeEmpty();
    }

    [Fact]
    public void ToDtoList_WithNull_ShouldReturnEmptyList()
    {
        // Arrange
        IEnumerable<Clients>? clients = null;

        // Act
        var dtos = UserMapper.ToDtoList(clients!);

        // Assert
        dtos.Should().NotBeNull();
        dtos.Should().BeEmpty();
    }

    [Fact]
    public void UpdateEntity_WithValidDto_ShouldUpdateAllProperties()
    {
        // Arrange
        var entity = new Clients
        {
            Id = 1,
            FirstName = "OldFirst",
            LastName = "OldLast",
            Email = "old@example.com",
            DateOfBirth = new DateOnly(1980, 1, 1),
            PhoneNumber = "0000000000",
            Address = "Old Address",
            Role = "Client"
        };

        var dto = new UpdateUserDto
        {
            Id = 1,
            FirstName = "NewFirst",
            LastName = "NewLast",
            Email = "new@example.com",
            DateOfBirth = new DateOnly(1990, 5, 15),
            PhoneNumber = "1234567890",
            Address = "New Address",
            Role = "Admin"
        };

        // Act
        UserMapper.UpdateEntity(dto, entity);

        // Assert
        entity.FirstName.Should().Be(dto.FirstName);
        entity.LastName.Should().Be(dto.LastName);
        entity.Email.Should().Be(dto.Email);
        entity.DateOfBirth.Should().Be(dto.DateOfBirth);
        entity.PhoneNumber.Should().Be(dto.PhoneNumber);
        entity.Address.Should().Be(dto.Address);
        entity.Role.Should().Be(dto.Role);
    }

    [Fact]
    public void UpdateEntity_WithNullDto_ShouldNotThrowException()
    {
        // Arrange
        var entity = new Clients { FirstName = "John" };
        UpdateUserDto? dto = null;

        // Act
        Action act = () => UserMapper.UpdateEntity(dto!, entity);

        // Assert
        act.Should().NotThrow();
        entity.FirstName.Should().Be("John"); // Entity should remain unchanged
    }

    [Fact]
    public void UpdateEntity_WithNullEntity_ShouldNotThrowException()
    {
        // Arrange
        var dto = new UpdateUserDto { FirstName = "John" };
        Clients? entity = null;

        // Act
        Action act = () => UserMapper.UpdateEntity(dto, entity!);

        // Assert
        act.Should().NotThrow();
    }
}
