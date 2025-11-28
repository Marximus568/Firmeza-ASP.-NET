using AdminDashboardApplication.Auth;
using AdminDashboardApplication.Auth.Interfaces;
using AdminDashboardApplication.Auth.UseCases;
using AdminDashboardApplication.DTOs.Users;
using FluentAssertions;
using Moq;
using Xunit;

namespace Firmeza.Test.Application.UseCases;

public class RegisterUserUseCaseTests
{
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly Mock<IRoleService> _mockRoleService;
    private readonly RegisterUserUseCase _registerUserUseCase;

    public RegisterUserUseCaseTests()
    {
        _mockAuthService = new Mock<IAuthService>();
        _mockRoleService = new Mock<IRoleService>();
        _registerUserUseCase = new RegisterUserUseCase(_mockAuthService.Object, _mockRoleService.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidData_ShouldRegisterSuccessfully()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Password = "SecurePassword123!",
            Role = "Client"
        };

        _mockAuthService.Setup(x => x.UserExistsAsync(registerDto.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var successResult = AuthResultDto.Success("user123", "john.doe@example.com", new[] { "Client" }, "token123");
        _mockAuthService.Setup(x => x.RegisterAsync(It.IsAny<UserDto>(), registerDto.Password, It.IsAny<CancellationToken>()))
            .ReturnsAsync(successResult);

        _mockRoleService.Setup(x => x.AssignRoleAsync("user123", "Client", It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _registerUserUseCase.ExecuteAsync(registerDto);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeTrue();
        result.UserId.Should().Be("user123");
        result.Token.Should().Be("token123");
        
        _mockAuthService.Verify(x => x.UserExistsAsync(registerDto.Email, It.IsAny<CancellationToken>()), Times.Once);
        _mockAuthService.Verify(x => x.RegisterAsync(It.IsAny<UserDto>(), registerDto.Password, It.IsAny<CancellationToken>()), Times.Once);
        _mockRoleService.Verify(x => x.AssignRoleAsync("user123", "Client", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithExistingEmail_ShouldReturnFailure()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "existing@example.com",
            Password = "Password123!"
        };

        _mockAuthService.Setup(x => x.UserExistsAsync(registerDto.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _registerUserUseCase.ExecuteAsync(registerDto);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeFalse();
        result.Errors.Should().Contain("A user with this email already exists");
        
        _mockAuthService.Verify(x => x.UserExistsAsync(registerDto.Email, It.IsAny<CancellationToken>()), Times.Once);
        _mockAuthService.Verify(x => x.RegisterAsync(It.IsAny<UserDto>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithNullRole_ShouldDefaultToClient()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane@example.com",
            Password = "Password123!",
            Role = null
        };

        _mockAuthService.Setup(x => x.UserExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var successResult = AuthResultDto.Success("user456", "jane@example.com", new[] { "Client" }, "token456");
        _mockAuthService.Setup(x => x.RegisterAsync(It.IsAny<UserDto>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(successResult);

        // Act
        var result = await _registerUserUseCase.ExecuteAsync(registerDto);

        // Assert
        result.Succeeded.Should().BeTrue();
        _mockRoleService.Verify(x => x.AssignRoleAsync("user456", "Client", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithEmptyRole_ShouldDefaultToClient()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            FirstName = "Bob",
            LastName = "Johnson",
            Email = "bob@example.com",
            Password = "Password123!",
            Role = "   " // Whitespace
        };

        _mockAuthService.Setup(x => x.UserExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var successResult = AuthResultDto.Success("user789", "bob@example.com", new[] { "Client" }, "token789");
        _mockAuthService.Setup(x => x.RegisterAsync(It.IsAny<UserDto>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(successResult);

        // Act
        var result = await _registerUserUseCase.ExecuteAsync(registerDto);

        // Assert
        result.Succeeded.Should().BeTrue();
        _mockRoleService.Verify(x => x.AssignRoleAsync("user789", "Client", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithAdminRole_ShouldAssignAdminRole()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            FirstName = "Admin",
            LastName = "User",
            Email = "admin@example.com",
            Password = "AdminPassword123!",
            Role = "Admin"
        };

        _mockAuthService.Setup(x => x.UserExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var successResult = AuthResultDto.Success("admin001", "admin@example.com", new[] { "Admin" }, "adminToken");
        _mockAuthService.Setup(x => x.RegisterAsync(It.IsAny<UserDto>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(successResult);

        // Act
        var result = await _registerUserUseCase.ExecuteAsync(registerDto);

        // Assert
        result.Succeeded.Should().BeTrue();
        _mockRoleService.Verify(x => x.AssignRoleAsync("admin001", "Admin", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenRegistrationFails_ShouldReturnFailure()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com",
            Password = "Password123!"
        };

        _mockAuthService.Setup(x => x.UserExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var failureResult = AuthResultDto.Failure("Registration failed due to database error");
        _mockAuthService.Setup(x => x.RegisterAsync(It.IsAny<UserDto>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        // Act
        var result = await _registerUserUseCase.ExecuteAsync(registerDto);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Errors.Should().Contain("Registration failed due to database error");
        
        // Role assignment should not be attempted if registration fails
        _mockRoleService.Verify(x => x.AssignRoleAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldPassCorrectDataToAuthService()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            Password = "Password123!",
            Role = "Client"
        };

        UserDto? capturedUserDto = null;
        string? capturedPassword = null;

        _mockAuthService.Setup(x => x.UserExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _mockAuthService.Setup(x => x.RegisterAsync(It.IsAny<UserDto>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Callback<UserDto, string, CancellationToken>((user, pwd, _) =>
            {
                capturedUserDto = user;
                capturedPassword = pwd;
            })
            .ReturnsAsync(AuthResultDto.Success("user123", "john@example.com", new[] { "Client" }, "token123"));

        // Act
        await _registerUserUseCase.ExecuteAsync(registerDto);

        // Assert
        capturedUserDto.Should().NotBeNull();
        capturedUserDto!.FirstName.Should().Be("John");
        capturedUserDto.LastName.Should().Be("Doe");
        capturedUserDto.Email.Should().Be("john@example.com");
        capturedUserDto.Role.Should().Be("Client");
        capturedPassword.Should().Be("Password123!");
    }

    [Fact]
    public async Task ExecuteAsync_WithEmptyUserId_ShouldNotAssignRole()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Email = "test@example.com",
            Password = "Password123!"
        };

        _mockAuthService.Setup(x => x.UserExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var successResultWithoutUserId = AuthResultDto.Success("", "test@example.com", new[] { "Client" }, "token123");
        _mockAuthService.Setup(x => x.RegisterAsync(It.IsAny<UserDto>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(successResultWithoutUserId);

        // Act
        await _registerUserUseCase.ExecuteAsync(registerDto);

        // Assert
        _mockRoleService.Verify(x => x.AssignRoleAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithCancellationToken_ShouldPassItThrough()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Email = "test@example.com",
            Password = "Password123!"
        };

        var cancellationToken = new CancellationToken();

        _mockAuthService.Setup(x => x.UserExistsAsync(It.IsAny<string>(), cancellationToken))
            .ReturnsAsync(false);

        _mockAuthService.Setup(x => x.RegisterAsync(It.IsAny<UserDto>(), It.IsAny<string>(), cancellationToken))
            .ReturnsAsync(AuthResultDto.Success("user123", "test@example.com", new[] { "Client" }, "token123"));

        // Act
        await _registerUserUseCase.ExecuteAsync(registerDto, cancellationToken);

        // Assert
        _mockAuthService.Verify(x => x.UserExistsAsync(It.IsAny<string>(), cancellationToken), Times.Once);
        _mockAuthService.Verify(x => x.RegisterAsync(It.IsAny<UserDto>(), It.IsAny<string>(), cancellationToken), Times.Once);
        _mockRoleService.Verify(x => x.AssignRoleAsync(It.IsAny<string>(), It.IsAny<string>(), cancellationToken), Times.Once);
    }
}
