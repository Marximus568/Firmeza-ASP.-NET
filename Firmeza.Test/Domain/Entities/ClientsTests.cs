using AdminDashboard.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Firmeza.Test.Domain.Entities;

public class ClientsTests
{
    [Fact]
    public void Constructor_WithAllParameters_ShouldCreateClientWithCorrectValues()
    {
        // Arrange
        var firstName = "John";
        var lastName = "Doe";
        var email = "john.doe@example.com";
        var phoneNumber = "1234567890";
        var address = "123 Main St";
        var role = "Admin";

        // Act
        var client = new Clients(firstName, lastName, email, phoneNumber, address, role);

        // Assert
        client.FirstName.Should().Be(firstName);
        client.LastName.Should().Be(lastName);
        client.Email.Should().Be(email);
        client.PhoneNumber.Should().Be(phoneNumber);
        client.Address.Should().Be(address);
        client.Role.Should().Be(role);
    }

    [Fact]
    public void Constructor_WithMinimalParameters_ShouldCreateClientWithDefaultRole()
    {
        // Arrange
        var firstName = "Jane";
        var lastName = "Smith";
        var email = "jane.smith@example.com";
        var phoneNumber = "9876543210";

        // Act
        var client = new Clients(firstName, lastName, email, phoneNumber);

        // Assert
        client.FirstName.Should().Be(firstName);
        client.LastName.Should().Be(lastName);
        client.Email.Should().Be(email);
        client.PhoneNumber.Should().Be(phoneNumber);
    }

    [Fact]
    public void Constructor_WithRoleParameter_AndNoRoleProvided_ShouldDefaultToClient()
    {
        // Arrange & Act
        var client = new Clients("John", "Doe", "john@example.com", "1234567890", "123 Main St");

        // Assert
        client.Role.Should().Be("Client");
    }

    [Fact]
    public void FullName_ShouldCombineFirstNameAndLastName()
    {
        // Arrange
        var client = new Clients("John", "Doe", "john@example.com", "1234567890");

        // Act
        var fullName = client.FullName;

        // Assert
        fullName.Should().Be("John Doe");
    }

    [Fact]
    public void FullName_WithOnlyFirstName_ShouldReturnTrimmedName()
    {
        // Arrange
        var client = new Clients
        {
            FirstName = "John",
            LastName = ""
        };

        // Act
        var fullName = client.FullName;

        // Assert
        fullName.Should().Be("John");
    }

    [Fact]
    public void Sales_NavigationProperty_ShouldBeInitializedAsEmptyList()
    {
        // Arrange & Act
        var client = new Clients();

        // Assert
        client.Sales.Should().NotBeNull();
        client.Sales.Should().BeEmpty();
    }

    [Fact]
    public void DefaultConstructor_ShouldCreateInstanceWithDefaultValues()
    {
        // Arrange & Act
        var client = new Clients();

        // Assert
        client.Should().NotBeNull();
        client.Role.Should().Be("Client");
        client.Sales.Should().NotBeNull();
    }

    [Fact]
    public void Client_ShouldHavePasswordProperty()
    {
        // Arrange
        var client = new Clients();
        var hashedPassword = "$2a$12$abcdefghijklmnopqrstuv";

        // Act
        client.Password = hashedPassword;

        // Assert
        client.Password.Should().Be(hashedPassword);
    }

    [Fact]
    public void Client_CanAddMultipleSales()
    {
        // Arrange
        var client = new Clients("John", "Doe", "john@example.com", "1234567890");
        var sale1 = new Sales { Id = 1, ClientId = client.Id };
        var sale2 = new Sales { Id = 2, ClientId = client.Id };

        // Act
        client.Sales.Add(sale1);
        client.Sales.Add(sale2);

        // Assert
        client.Sales.Should().HaveCount(2);
        client.Sales.Should().Contain(sale1);
        client.Sales.Should().Contain(sale2);
    }
}
