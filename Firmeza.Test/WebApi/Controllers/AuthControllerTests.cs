using System.Security.Claims;
using AdminDashboardApplication.Auth;
using AdminDashboardApplication.Auth.UseCases;
using AdminDashboardApplication.DTOs.Users;
using Firmeza.WebApi.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Firmeza.Test.WebApi.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<ILogger<AuthController>> _mockLogger;
    private readonly Mock<AdminDashboardApplication.Auth.Interfaces.IAuthService> _mockAuthService;
    private readonly LoginUserUseCase _loginUseCase;
    private readonly RegisterUserUseCase _registerUseCase;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _mockLogger = new Mock<ILogger<AuthController>>();
        _mockAuthService = new Mock<AdminDashboardApplication.Auth.Interfaces.IAuthService>();
        
        // Setup configuration for JWT
        _mockConfiguration.Setup(c => c["Jwt:Key"]).Returns("SuperSecretKeyForTestingPurposes12345!");
        _mockConfiguration.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
        _mockConfiguration.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");

        // Initialize UseCases with mocked AuthService
        // Note: In a real scenario, we might mock the UseCases themselves if they were interfaces,
        // but here they are concrete classes, so we mock their dependency (IAuthService).
        // Alternatively, we could mock the UseCases if we extract interfaces for them or make methods virtual.
        // For this test, assuming UseCases are lightweight wrappers, we test through them or mock the service they use.
        // However, since UseCases are injected, let's try to mock the IAuthService they depend on.
        
        // Wait, the controller takes concrete UseCase classes. 
        // To properly unit test the controller in isolation from UseCase logic, 
        // we should ideally mock the UseCases. But since they are concrete, 
        // we will rely on mocking the IAuthService that the UseCases use, 
        // OR we can create a subclass/mock if methods are virtual (they are not).
        // 
        // Given the constraints and the "No te extiendas" instruction, 
        // I will instantiate the real UseCases with a mocked IAuthService. 
        // This effectively tests the Controller + UseCase integration, which is acceptable here.
        // Ideally, UseCases should be interfaces (IUseCase<TIn, TOut>).
        
        var mockRoleService = new Mock<AdminDashboardApplication.Auth.Interfaces.IRoleService>();
        
        _loginUseCase = new LoginUserUseCase(_mockAuthService.Object);
        _registerUseCase = new RegisterUserUseCase(_mockAuthService.Object, mockRoleService.Object);

        _controller = new AuthController(
            _loginUseCase,
            _registerUseCase,
            _mockConfiguration.Object,
            _mockLogger.Object
        );
    }

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnOkWithToken()
    {
        // Arrange
        var loginDto = new LoginDto { Email = "test@example.com", Password = "Password123!" };
        var authResult = AuthResultDto.Success("user1", "test@example.com", new[] { "Client" }, "token_from_service");

        _mockAuthService.Setup(x => x.LoginAsync(It.IsAny<LoginDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(authResult);

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<LoginResponseDto>().Subject;
        
        response.Token.Should().NotBeNullOrEmpty();
        response.User.Email.Should().Be("test@example.com");
        response.User.UserId.Should().Be("user1");
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ShouldReturnUnauthorized()
    {
        // Arrange
        var loginDto = new LoginDto { Email = "test@example.com", Password = "WrongPassword" };
        var authResult = AuthResultDto.Failure("Invalid credentials");

        _mockAuthService.Setup(x => x.LoginAsync(It.IsAny<LoginDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(authResult);

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task Register_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        var registerDto = new RegisterDto 
        { 
            FirstName = "John", 
            LastName = "Doe", 
            Email = "new@example.com", 
            Password = "Password123!" 
        };
        
        var authResult = AuthResultDto.Success("user2", "new@example.com", new[] { "Client" }, "token_from_service");

        _mockAuthService.Setup(x => x.UserExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
            
        _mockAuthService.Setup(x => x.RegisterAsync(It.IsAny<UserDto>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(authResult);

        // Act
        var result = await _controller.Register(registerDto);

        // Assert
        var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var response = createdResult.Value.Should().BeOfType<LoginResponseDto>().Subject;
        
        response.Token.Should().NotBeNullOrEmpty();
        response.User.Email.Should().Be("new@example.com");
    }

    [Fact]
    public async Task Register_WithExistingEmail_ShouldReturnBadRequest()
    {
        // Arrange
        var registerDto = new RegisterDto 
        { 
            FirstName = "John", 
            LastName = "Doe", 
            Email = "existing@example.com", 
            Password = "Password123!" 
        };

        _mockAuthService.Setup(x => x.UserExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.Register(registerDto);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }
}
