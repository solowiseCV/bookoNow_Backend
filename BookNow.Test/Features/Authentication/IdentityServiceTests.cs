//using BookNow.Application.DTOs.Authentication.Request;
//using BookNow.Application.DTOs.Authentication.Response;
//using BookNow.Application.Interfaces.Authentication;
//using BookNow.Application.Interfaces.Persistence;
//using BookNow.Application.Interfaces.Services;
//using BookNow.Domain.Entities;
//using BookNow.Domain.Enums;
//using BookNow.Infrastructure.Identity;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;
//using Microsoft.AspNetCore.WebUtilities;
//using System.Text;
//using Xunit;
//using Moq;

//namespace BookNow.Test.Features.Authentication;

//public class IdentityServiceTests
//{
//    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
//    private readonly Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock;
//    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
//    private readonly Mock<IEmailService> _emailServiceMock;
//    private readonly Mock<ISmsService> _smsServiceMock;
//    private readonly Mock<ILogger<IdentityService>> _loggerMock;
//    private readonly Mock<IOptions<GoogleAuthOptions>> _googleOptionsMock;
//    private readonly Mock<IOptions<JwtSettings>> _jwtSettingsMock;
//    private readonly IdentityService _identityService;

//    public IdentityServiceTests()
//    {
//        _userManagerMock = new Mock<UserManager<ApplicationUser>>(
//            Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
//        _jwtTokenGeneratorMock = new Mock<IJwtTokenGenerator>();
//        _unitOfWorkMock = new Mock<IUnitOfWork>();
//        _emailServiceMock = new Mock<IEmailService>();
//        _smsServiceMock = new Mock<ISmsService>();
//        _loggerMock = new Mock<ILogger<IdentityService>>();
        
//        _googleOptionsMock = new Mock<IOptions<GoogleAuthOptions>>();
//        _googleOptionsMock.Setup(x => x.Value).Returns(new GoogleAuthOptions { ClientId = "test-client-id" });

//        _jwtSettingsMock = new Mock<IOptions<JwtSettings>>();
//        _jwtSettingsMock.Setup(x => x.Value).Returns(new JwtSettings { Secret = "supersecretkey12345678901234567890", ExpiryMinutes = 60, Issuer = "test", Audience = "test" });

//        _identityService = new IdentityService(
//            _userManagerMock.Object,
//            _jwtTokenGeneratorMock.Object,
//            _unitOfWorkMock.Object,
//            _emailServiceMock.Object,
//            _smsServiceMock.Object,
//            _loggerMock.Object,
//            _googleOptionsMock.Object,
//            _jwtSettingsMock.Object);
//    }

//    [Fact]
//    public async Task LoginWithGoogleAsync_EmptyToken_ReturnsFailure()
//    {
//        // Arrange
//        var request = new GoogleAuthRequestDto("");

//        // Act
//        var result = await _identityService.LoginWithGoogleAsync(request);

//        // Assert
//        Assert.False(result.Success);
//        Assert.Contains("IdToken is required", result.Errors);
//    }

//    [Fact]
//    public async Task ForgotPasswordAsync_EmptyEmail_ReturnsFailure()
//    {
//        // Arrange
//        var request = new ForgotPasswordRequestDto("");

//        // Act
//        var result = await _identityService.ForgotPasswordAsync(request);

//        // Assert
//        Assert.True(result.Success);
//        Assert.Equal("If the email exists, a reset link has been sent.", result.Message);
//    }

//    [Fact]
//    public async Task ForgotPasswordAsync_NonExistentUser_ReturnsSuccessWithGenericMessage()
//    {
//        // Arrange
//        var request = new ForgotPasswordRequestDto("nonexistent@example.com");
//        _userManagerMock.Setup(x => x.FindByEmailAsync(request.Email)).ReturnsAsync((ApplicationUser)null!);

//        // Act
//        var result = await _identityService.ForgotPasswordAsync(request);

//        // Assert
//        Assert.True(result.Success);
//        Assert.Equal("If the email exists, a reset link has been sent.", result.Message);
//    }

//    [Fact]
//    public async Task ResetPasswordAsync_ValidRequest_ReturnsSuccess()
//    {
//        // Arrange
//        var request = new ResetPasswordRequestDto("user@example.com", WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes("valid-token")), "NewPassword123!");
//        var user = new ApplicationUser { Email = request.Email };
//        _userManagerMock.Setup(x => x.FindByEmailAsync(request.Email)).ReturnsAsync(user);
//        _userManagerMock.Setup(x => x.ResetPasswordAsync(user, "valid-token", request.NewPassword))
//            .ReturnsAsync(IdentityResult.Success);

//        // Act
//        var result = await _identityService.ResetPasswordAsync(request);

//        // Assert
//        Assert.True(result.Success);
//        Assert.Equal("Password has been reset successfully.", result.Message);
//    }

//    [Fact]
//    public async Task ResetPasswordAsync_InvalidToken_ReturnsFailure()
//    {
//        // Arrange
//        var request = new ResetPasswordRequestDto("user@example.com", WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes("invalid-token")), "NewPassword123!");
//        var user = new ApplicationUser { Email = request.Email };
//        var identityResult = IdentityResult.Failed(new IdentityError { Description = "Invalid token" });
//        _userManagerMock.Setup(x => x.FindByEmailAsync(request.Email)).ReturnsAsync(user);
//        _userManagerMock.Setup(x => x.ResetPasswordAsync(user, "invalid-token", request.NewPassword))
//            .ReturnsAsync(identityResult);

//        // Act
//        var result = await _identityService.ResetPasswordAsync(request);

//        // Assert
//        Assert.False(result.Success);
//        Assert.Contains("Invalid token", result.Errors);
//    }

//    [Fact]
//    public async Task ResetPasswordAsync_MissingFields_ReturnsFailure()
//    {
//        // Arrange
//        var request = new ResetPasswordRequestDto("", "token", "password");

//        // Act
//        var result = await _identityService.ResetPasswordAsync(request);

//        // Assert
//        Assert.False(result.Success);
//        Assert.Contains("All fields are required", result.Errors);
//    }

//    [Fact]
//    public async Task ChangePasswordAsync_ValidRequest_ReturnsSuccess()
//    {
//        // Arrange
//        var request = new ChangePasswordRequestDto("OldPass123!", "NewPass123!");
//        var user = new ApplicationUser { Id = Guid.NewGuid(), Email = "user@example.com" };
//        var userId = "user-id-123";
//        _userManagerMock.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(user);
//        _userManagerMock.Setup(x => x.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword))
//            .ReturnsAsync(IdentityResult.Success);

//        // Act
//        var result = await _identityService.ChangePasswordAsync(userId, request);

//        // Assert
//        Assert.True(result.Success);
//        Assert.Equal("Password changed successfully", result.Message);
//    }

//    [Fact]
//    public async Task ChangePasswordAsync_InvalidCurrentPassword_ReturnsFailure()
//    {
//        // Arrange
//        var request = new ChangePasswordRequestDto("WrongOld", "NewPass123!");
//        var user = new ApplicationUser { Id = Guid.NewGuid(), Email = "user@example.com" };
//        var userId = "user-id-123";
//        var identityResult = IdentityResult.Failed(new IdentityError { Description = "Incorrect password" });
//        _userManagerMock.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(user);
//        _userManagerMock.Setup(x => x.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword))
//            .ReturnsAsync(identityResult);

//        // Act
//        var result = await _identityService.ChangePasswordAsync(userId, request);

//        // Assert
//        Assert.False(result.Success);
//        Assert.Contains("Incorrect password", result.Errors);
//    }

//    [Fact]
//    public async Task ChangePasswordAsync_MissingFields_ReturnsFailure()
//    {
//        // Arrange
//        var request = new ChangePasswordRequestDto("", "");
//        var userId = "user-id-123";

//        // Act
//        var result = await _identityService.ChangePasswordAsync(userId, request);

//        // Assert
//        Assert.False(result.Success);
//        Assert.Contains("Current and new passwords are required", result.Errors);
//    }
//}
