using AdminDashboardApplication.Auth;
using AdminDashboardApplication.Auth.Interfaces;
using AdminDashboardApplication.Auth.UseCases;
using FluentAssertions;
using Moq;
using Xunit;

namespace Firmeza.Test.Application.UseCases;

public class LoginUserUseCaseTests
{
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly LoginUserUseCase _loginUserUseCase;

    public LoginUserUseCaseTests()
    {
        _mockAuthService = new Mock<IAuthService>();
        _loginUserUseCase = new LoginUserUseCase(_mockAuthService.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidCredentials_ShouldReturnSuccess()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "john.doe@example.com",
            Password = "CorrectPassword123!"
        };

        var successResult = AuthResultDto.Success("user123", "john.doe@example.com", new[] { "Client" }, "token123");
        _mockAuthService.Setup(x => x.LoginAsync(loginDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(successResult);

        // Act
        var result = await _loginUserUseCase.ExecuteAsync(loginDto);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeTrue();
        result.UserId.Should().Be("user123");
        result.Token.Should().Be("token123");
        
        _mockAuthService.Verify(x => x.LoginAsync(loginDto, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidCredentials_ShouldReturnFailure()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "john.doe@example.com",
            Password = "WrongPassword"
        };

        var failureResult = AuthResultDto.Failure("Invalid email or password");
        _mockAuthService.Setup(x => x.LoginAsync(loginDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        // Act
        var result = await _loginUserUseCase.ExecuteAsync(loginDto);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeFalse();
        result.Errors.Should().Contain("Invalid email or password");
        
        _mockAuthService.Verify(x => x.LoginAsync(loginDto, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithEmptyEmail_ShouldReturnFailure()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "",
            Password = "SomePassword123!"
        };

        // Act
        var result = await _loginUserUseCase.ExecuteAsync(loginDto);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeFalse();
        result.Errors.Should().Contain("Email is required.");
        
        _mockAuthService.Verify(x => x.LoginAsync(It.IsAny<LoginDto>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithNullEmail_ShouldReturnFailure()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = null!,
            Password = "SomePassword123!"
        };

        // Act
        var result = await _loginUserUseCase.ExecuteAsync(loginDto);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeFalse();
        result.Errors.Should().Contain("Email is required.");
        
        _mockAuthService.Verify(x => x.LoginAsync(It.IsAny<LoginDto>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithWhitespaceEmail_ShouldReturnFailure()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "   ",
            Password = "SomePassword123!"
        };

        // Act
        var result = await _loginUserUseCase.ExecuteAsync(loginDto);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeFalse();
        result.Errors.Should().Contain("Email is required.");
        
        _mockAuthService.Verify(x => x.LoginAsync(It.IsAny<LoginDto>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithEmptyPassword_ShouldReturnFailure()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "john.doe@example.com",
            Password = ""
        };

        // Act
        var result = await _loginUserUseCase.ExecuteAsync(loginDto);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeFalse();
        result.Errors.Should().Contain("Password is required.");
        
        _mockAuthService.Verify(x => x.LoginAsync(It.IsAny<LoginDto>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithNullPassword_ShouldReturnFailure()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "john.doe@example.com",
            Password = null!
        };

        // Act
        var result = await _loginUserUseCase.ExecuteAsync(loginDto);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeFalse();
        result.Errors.Should().Contain("Password is required.");
        
        _mockAuthService.Verify(x => x.LoginAsync(It.IsAny<LoginDto>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithWhitespacePassword_ShouldReturnFailure()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "john.doe@example.com",
            Password = "   "
        };

        // Act
        var result = await _loginUserUseCase.ExecuteAsync(loginDto);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeFalse();
        result.Errors.Should().Contain("Password is required.");
        
        _mockAuthService.Verify(x => x.LoginAsync(It.IsAny<LoginDto>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidInput_ShouldDelegateToAuthService()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "test@example.com",
            Password = "Password123!"
        };

        var authResult = AuthResultDto.Success("user456", "test@example.com", new[] { "Client" }, "token456");
        _mockAuthService.Setup(x => x.LoginAsync(loginDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(authResult);

        // Act
        var result = await _loginUserUseCase.ExecuteAsync(loginDto);

        // Assert
        result.Should().Be(authResult);
        _mockAuthService.Verify(x => x.LoginAsync(loginDto, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithCancellationToken_ShouldPassItToAuthService()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "test@example.com",
            Password = "Password123!"
        };

        var cancellationToken = new CancellationToken();

        _mockAuthService.Setup(x => x.LoginAsync(loginDto, cancellationToken))
            .ReturnsAsync(AuthResultDto.Success("user789", "test@example.com", new[] { "Client" }, "token789"));

        // Act
        await _loginUserUseCase.ExecuteAsync(loginDto, cancellationToken);

        // Assert
        _mockAuthService.Verify(x => x.LoginAsync(loginDto, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenAuthServiceReturnsFailure_ShouldReturnThatFailure()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "locked@example.com",
            Password = "Password123!"
        };

        var failureResult = AuthResultDto.Failure("Account is locked");
        _mockAuthService.Setup(x => x.LoginAsync(loginDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        // Act
        var result = await _loginUserUseCase.ExecuteAsync(loginDto);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeFalse();
        result.Errors.Should().Contain("Account is locked");
    }

    [Fact]
    public async Task ExecuteAsync_ShouldValidateInputBeforeDelegating()
    {
        // Arrange
        var invalidLoginDto = new LoginDto
        {
            Email = "",
            Password = ""
        };

        // Act
        await _loginUserUseCase.ExecuteAsync(invalidLoginDto);

        // Assert
        // Should not call auth service if validation fails
        _mockAuthService.Verify(x => x.LoginAsync(It.IsAny<LoginDto>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithBothValidInputs_ShouldNotReturnValidationError()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "valid@example.com",
            Password = "ValidPassword123!"
        };

        _mockAuthService.Setup(x => x.LoginAsync(loginDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(AuthResultDto.Success("user999", "valid@example.com", new[] { "Client" }, "token999"));

        // Act
        var result = await _loginUserUseCase.ExecuteAsync(loginDto);

        // Assert
        result.Succeeded.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }
}
