using AdminDashboard.Infrastructure.Services.PasswordHasher;
using FluentAssertions;
using Xunit;

namespace Firmeza.Test.Infrastructure.Services;

public class PasswordHasherTests
{
    private readonly PasswordHasher _passwordHasher;

    public PasswordHasherTests()
    {
        _passwordHasher = new PasswordHasher();
    }

    [Fact]
    public void HashPassword_WithValidPassword_ShouldReturnNonEmptyHash()
    {
        // Arrange
        var password = "MySecurePassword123!";

        // Act
        var hash = _passwordHasher.HashPassword(password);

        // Assert
        hash.Should().NotBeNullOrEmpty();
        hash.Should().NotBe(password); // Hash should be different from plain text
    }

    [Fact]
    public void HashPassword_ShouldProduceDifferentHashesForSamePassword()
    {
        // Arrange
        var password = "SamePassword123";

        // Act
        var hash1 = _passwordHasher.HashPassword(password);
        var hash2 = _passwordHasher.HashPassword(password);

        // Assert
        hash1.Should().NotBe(hash2); // BCrypt uses salt, so hashes should be different
    }

    [Fact]
    public void HashPassword_WithNullPassword_ShouldThrowArgumentNullException()
    {
        // Arrange
        string? password = null;

        // Act
        Action act = () => _passwordHasher.HashPassword(password!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("*password*");
    }

    [Fact]
    public void HashPassword_WithEmptyPassword_ShouldThrowArgumentNullException()
    {
        // Arrange
        var password = "";

        // Act
        Action act = () => _passwordHasher.HashPassword(password);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("*password*");
    }

    [Fact]
    public void HashPassword_WithWhitespacePassword_ShouldThrowArgumentNullException()
    {
        // Arrange
        var password = "   ";

        // Act
        Action act = () => _passwordHasher.HashPassword(password);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("*password*");
    }

    [Fact]
    public void VerifyPassword_WithCorrectPassword_ShouldReturnTrue()
    {
        // Arrange
        var password = "CorrectPassword123!";
        var hash = _passwordHasher.HashPassword(password);

        // Act
        var result = _passwordHasher.VerifyPassword(password, hash);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_WithIncorrectPassword_ShouldReturnFalse()
    {
        // Arrange
        var correctPassword = "CorrectPassword123!";
        var incorrectPassword = "WrongPassword456!";
        var hash = _passwordHasher.HashPassword(correctPassword);

        // Act
        var result = _passwordHasher.VerifyPassword(incorrectPassword, hash);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void VerifyPassword_WithNullPassword_ShouldReturnFalse()
    {
        // Arrange
        var hash = _passwordHasher.HashPassword("SomePassword");
        string? password = null;

        // Act
        var result = _passwordHasher.VerifyPassword(password!, hash);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void VerifyPassword_WithNullHash_ShouldReturnFalse()
    {
        // Arrange
        var password = "SomePassword";
        string? hash = null;

        // Act
        var result = _passwordHasher.VerifyPassword(password, hash!);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void VerifyPassword_WithEmptyPassword_ShouldReturnFalse()
    {
        // Arrange
        var hash = _passwordHasher.HashPassword("SomePassword");
        var password = "";

        // Act
        var result = _passwordHasher.VerifyPassword(password, hash);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void VerifyPassword_WithEmptyHash_ShouldReturnFalse()
    {
        // Arrange
        var password = "SomePassword";
        var hash = "";

        // Act
        var result = _passwordHasher.VerifyPassword(password, hash);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void VerifyPassword_WithInvalidHash_ShouldReturnFalse()
    {
        // Arrange
        var password = "SomePassword";
        var invalidHash = "this-is-not-a-valid-bcrypt-hash";

        // Act
        var result = _passwordHasher.VerifyPassword(password, invalidHash);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void HashPassword_ShouldProduceBCryptFormattedHash()
    {
        // Arrange
        var password = "TestPassword123";

        // Act
        var hash = _passwordHasher.HashPassword(password);

        // Assert
        hash.Should().StartWith("$2a$"); // BCrypt hashes start with $2a$
        hash.Length.Should().Be(60); // BCrypt hashes are 60 characters long
    }

    [Fact]
    public void VerifyPassword_CaseSensitive_ShouldReturnFalse()
    {
        // Arrange
        var password = "Password123";
        var hash = _passwordHasher.HashPassword(password);
        var wrongCasePassword = "password123";

        // Act
        var result = _passwordHasher.VerifyPassword(wrongCasePassword, hash);

        // Assert
        result.Should().BeFalse(); // Passwords should be case-sensitive
    }

    [Fact]
    public void HashPassword_WithSpecialCharacters_ShouldHashCorrectly()
    {
        // Arrange
        var password = "P@ssw0rd!#$%^&*()";

        // Act
        var hash = _passwordHasher.HashPassword(password);
        var verified = _passwordHasher.VerifyPassword(password, hash);

        // Assert
        hash.Should().NotBeNullOrEmpty();
        verified.Should().BeTrue();
    }

    [Fact]
    public void HashPassword_WithLongPassword_ShouldHashCorrectly()
    {
        // Arrange
        var password = new string('a', 100); // Very long password

        // Act
        var hash = _passwordHasher.HashPassword(password);
        var verified = _passwordHasher.VerifyPassword(password, hash);

        // Assert
        hash.Should().NotBeNullOrEmpty();
        verified.Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_WithSimilarButNotIdenticalPassword_ShouldReturnFalse()
    {
        // Arrange
        var password = "Password123";
        var hash = _passwordHasher.HashPassword(password);
        var similarPassword = "Password124"; // Only last character different

        // Act
        var result = _passwordHasher.VerifyPassword(similarPassword, hash);

        // Assert
        result.Should().BeFalse();
    }
}
