using AdminDashboard.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Firmeza.Test.Domain.Entities;

public class SalesTests
{
    [Fact]
    public void Sale_ShouldBeCreatedWithDefaultValues()
    {
        // Arrange & Act
        var sale = new Sales();

        // Assert
        sale.Should().NotBeNull();
        sale.SaleDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        sale.TaxRate.Should().Be(0.19m);
        sale.IsPaid.Should().BeTrue();
        sale.Items.Should().NotBeNull();
        sale.Items.Should().BeEmpty();
    }

    [Fact]
    public void Sale_WithClientId_ShouldSetCorrectly()
    {
        // Arrange
        var clientId = 42;

        // Act
        var sale = new Sales { ClientId = clientId };

        // Assert
        sale.ClientId.Should().Be(clientId);
    }

    [Fact]
    public void Sale_Subtotal_ShouldBeSetCorrectly()
    {
        // Arrange
        var subtotal = 500.50m;

        // Act
        var sale = new Sales { Subtotal = subtotal };

        // Assert
        sale.Subtotal.Should().Be(subtotal);
    }

    [Fact]
    public void Sale_TaxRate_ShouldDefaultTo19Percent()
    {
        // Arrange & Act
        var sale = new Sales();

        // Assert
        sale.TaxRate.Should().Be(0.19m);
    }

    [Fact]
    public void Sale_TaxRate_CanBeCustomized()
    {
        // Arrange
        var customTaxRate = 0.21m;

        // Act
        var sale = new Sales { TaxRate = customTaxRate };

        // Assert
        sale.TaxRate.Should().Be(customTaxRate);
    }

    [Fact]
    public void Sale_Total_ShouldBeSetCorrectly()
    {
        // Arrange
        var total = 595.60m;

        // Act
        var sale = new Sales { Total = total };

        // Assert
        sale.Total.Should().Be(total);
    }

    [Fact]
    public void Sale_CalculateTotalFromSubtotalAndTax_ShouldBeCorrect()
    {
        // Arrange
        var subtotal = 500m;
        var taxRate = 0.19m;
        var expectedTotal = 595m; // 500 + (500 * 0.19)

        // Act
        var sale = new Sales
        {
            Subtotal = subtotal,
            TaxRate = taxRate,
            Total = subtotal + (subtotal * taxRate)
        };

        // Assert
        sale.Total.Should().Be(expectedTotal);
    }

    [Fact]
    public void Sale_IsPaid_ShouldDefaultToTrue()
    {
        // Arrange & Act
        var sale = new Sales();

        // Assert
        sale.IsPaid.Should().BeTrue();
    }

    [Fact]
    public void Sale_IsPaid_CanBeSetToFalse()
    {
        // Arrange & Act
        var sale = new Sales { IsPaid = false };

        // Assert
        sale.IsPaid.Should().BeFalse();
    }

    [Fact]
    public void Sale_PaymentMethod_CanBeSet()
    {
        // Arrange
        var paymentMethod = "Credit Card";

        // Act
        var sale = new Sales { PaymentMethod = paymentMethod };

        // Assert
        sale.PaymentMethod.Should().Be(paymentMethod);
    }

    [Fact]
    public void Sale_InvoiceNumber_CanBeSet()
    {
        // Arrange
        var invoiceNumber = "INV-2025-001";

        // Act
        var sale = new Sales { InvoiceNumber = invoiceNumber };

        // Assert
        sale.InvoiceNumber.Should().Be(invoiceNumber);
    }

    [Fact]
    public void Sale_Notes_CanBeSet()
    {
        // Arrange
        var notes = "Customer requested express delivery";

        // Act
        var sale = new Sales { Notes = notes };

        // Assert
        sale.Notes.Should().Be(notes);
    }

    [Fact]
    public void Sale_ReceiptPath_CanBeNull()
    {
        // Arrange & Act
        var sale = new Sales();

        // Assert
        sale.ReceiptPath.Should().BeNullOrEmpty();
    }

    [Fact]
    public void Sale_ReceiptPath_CanBeSet()
    {
        // Arrange
        var receiptPath = "/wwwroot/recibos/receipt_001.pdf";

        // Act
        var sale = new Sales { ReceiptPath = receiptPath };

        // Assert
        sale.ReceiptPath.Should().Be(receiptPath);
    }

    [Fact]
    public void Sale_Items_NavigationProperty_ShouldBeInitializedAsEmptyList()
    {
        // Arrange & Act
        var sale = new Sales();

        // Assert
        sale.Items.Should().NotBeNull();
        sale.Items.Should().BeEmpty();
    }

    [Fact]
    public void Sale_CanAddMultipleSaleItems()
    {
        // Arrange
        var sale = new Sales { Id = 1 };
        var item1 = new SaleItems { SalesId = sale.Id, ProductId = 1, Quantity = 2, UnitPrice = 50m };
        var item2 = new SaleItems { SalesId = sale.Id, ProductId = 2, Quantity = 3, UnitPrice = 30m };

        // Act
        sale.Items.Add(item1);
        sale.Items.Add(item2);

        // Assert
        sale.Items.Should().HaveCount(2);
        sale.Items.Should().Contain(item1);
        sale.Items.Should().Contain(item2);
    }

    [Fact]
    public void Sale_Clients_NavigationProperty_CanBeSet()
    {
        // Arrange
        var client = new Clients("John", "Doe", "john@example.com", "1234567890");
        var sale = new Sales { ClientId = 1, Clients = client };

        // Act & Assert
        sale.Clients.Should().NotBeNull();
        sale.Clients.Should().Be(client);
    }

    [Fact]
    public void Sale_SaleDate_ShouldBeSetOnCreation()
    {
        // Arrange
        var beforeCreation = DateTime.UtcNow.AddSeconds(-1);

        // Act
        var sale = new Sales();
        var afterCreation = DateTime.UtcNow.AddSeconds(1);

        // Assert
        sale.SaleDate.Should().BeAfter(beforeCreation);
        sale.SaleDate.Should().BeBefore(afterCreation);
    }

    [Fact]
    public void Sale_WithZeroTaxRate_ShouldHaveTotalEqualToSubtotal()
    {
        // Arrange
        var subtotal = 100m;
        var taxRate = 0m;

        // Act
        var sale = new Sales
        {
            Subtotal = subtotal,
            TaxRate = taxRate,
            Total = subtotal + (subtotal * taxRate)
        };

        // Assert
        sale.Total.Should().Be(subtotal);
    }
}
