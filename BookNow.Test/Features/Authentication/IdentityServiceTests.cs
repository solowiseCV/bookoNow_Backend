using BookNow.Application.DTOs.Authentication.Request;
using BookNow.Application.DTOs.Authentication.Response;
using BookNow.Application.Interfaces.Authentication;
using BookNow.Application.Interfaces.Persistence;
using BookNow.Application.Interfaces.Services;
using BookNow.Domain.Entities;
using BookNow.Domain.Enums;
using BookNow.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace BookNow.Test.Features.Authentication;

[TestClass]
public class IdentityServiceTests
{
    private Mock<UserManager<ApplicationUser>> _userManagerMock;
    private Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock;
    private Mock<IUnitOfWork> _unitOfWorkMock;
    private Mock<IEmailService> _emailServiceMock;
    private Mock<ILogger<IdentityService>> _loggerMock;
    private Mock<IOptions<GoogleAuthOptions>> _googleOptionsMock;
    private IdentityService _identityService;

    [TestInitialize]
    public void Setup()
    {
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
        _jwtTokenGeneratorMock = new Mock<IJwtTokenGenerator>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _emailServiceMock = new Mock<IEmailService>();
        _loggerMock = new Mock<ILogger<IdentityService>>();
        _googleOptionsMock = new Mock<IOptions<GoogleAuthOptions>>();
        _googleOptionsMock.Setup(x => x.Value).Returns(new GoogleAuthOptions { ClientId = "test-client-id" });

        _identityService = new IdentityService(
            _userManagerMock.Object,
            _jwtTokenGeneratorMock.Object,
            _unitOfWorkMock.Object,
            _emailServiceMock.Object,
            _loggerMock.Object,
            _googleOptionsMock.Object);
    }

    [TestMethod]
    public async Task LoginWithGoogleAsync_EmptyToken_ReturnsFailure()
    {
        // Arrange
        var request = new GoogleAuthRequestDto("");

        // Act
        var result = await _identityService.LoginWithGoogleAsync(request);

        // Assert
        Assert.IsFalse(result.Success);
        Assert.IsTrue(result.Errors.Contains("IdToken is required"));
    }

    [TestMethod]
    public async Task ForgotPasswordAsync_EmptyEmail_ReturnsFailure()
    {
        // Arrange
        var request = new ForgotPasswordRequestDto("");

        // Act
        var result = await _identityService.ForgotPasswordAsync(request);

        // Assert
        Assert.IsFalse(result.Success);
        Assert.IsTrue(result.Errors.Contains("Email is required"));
    }

    [TestMethod]
    public async Task ForgotPasswordAsync_NonExistentUser_ReturnsSuccessWithGenericMessage()
    {
        // Arrange
        var request = new ForgotPasswordRequestDto("nonexistent@example.com");
        _userManagerMock.Setup(x => x.FindByEmailAsync(request.Email)).ReturnsAsync((ApplicationUser)null);

        // Act
        var result = await _identityService.ForgotPasswordAsync(request);

        // Assert
        Assert.IsTrue(result.Success);
        Assert.AreEqual("If that email is registered, a password reset link has been sent.", result.Message);
    }

    [TestMethod]
    public async Task ResetPasswordAsync_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var request = new ResetPasswordRequestDto("user@example.com", "valid-token", "NewPassword123!");
        var user = new ApplicationUser { Email = request.Email };
        _userManagerMock.Setup(x => x.FindByEmailAsync(request.Email)).ReturnsAsync(user);
        _userManagerMock.Setup(x => x.ResetPasswordAsync(user, request.Token, request.NewPassword))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _identityService.ResetPasswordAsync(request);

        // Assert
        Assert.IsTrue(result.Success);
        Assert.AreEqual("Password has been reset successfully.", result.Message);
    }

    [TestMethod]
    public async Task ResetPasswordAsync_InvalidToken_ReturnsFailure()
    {
        // Arrange
        var request = new ResetPasswordRequestDto("user@example.com", "invalid-token", "NewPassword123!");
        var user = new ApplicationUser { Email = request.Email };
        var identityResult = IdentityResult.Failed(new IdentityError { Description = "Invalid token" });
        _userManagerMock.Setup(x => x.FindByEmailAsync(request.Email)).ReturnsAsync(user);
        _userManagerMock.Setup(x => x.ResetPasswordAsync(user, request.Token, request.NewPassword))
            .ReturnsAsync(identityResult);

        // Act
        var result = await _identityService.ResetPasswordAsync(request);

        // Assert
        Assert.IsFalse(result.Success);
        Assert.IsTrue(result.Errors.Contains("Invalid token"));
    }

    [TestMethod]
    public async Task ResetPasswordAsync_MissingFields_ReturnsFailure()
    {
        // Arrange
        var request = new ResetPasswordRequestDto("", "token", "password");

        // Act
        var result = await _identityService.ResetPasswordAsync(request);

        // Assert
        Assert.IsFalse(result.Success);
        Assert.IsTrue(result.Errors.Contains("All fields are required"));
    }

    [TestMethod]
    public async Task ChangePasswordAsync_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var request = new ChangePasswordRequestDto("user-id-123", "OldPass123!", "NewPass123!");
        var user = new ApplicationUser { Id = Guid.NewGuid(), Email = "user@example.com" };
        _userManagerMock.Setup(x => x.FindByIdAsync(request.UserId)).ReturnsAsync(user);
        _userManagerMock.Setup(x => x.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _identityService.ChangePasswordAsync(request);

        // Assert
        Assert.IsTrue(result.Success);
        Assert.AreEqual("Password changed successfully.", result.Message);
    }

    [TestMethod]
    public async Task ChangePasswordAsync_InvalidCurrentPassword_ReturnsFailure()
    {
        // Arrange
        var request = new ChangePasswordRequestDto("user-id-123", "WrongOld", "NewPass123!");
        var user = new ApplicationUser { Id = Guid.NewGuid(), Email = "user@example.com" };
        var identityResult = IdentityResult.Failed(new IdentityError { Description = "Incorrect password" });
        _userManagerMock.Setup(x => x.FindByIdAsync(request.UserId)).ReturnsAsync(user);
        _userManagerMock.Setup(x => x.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword))
            .ReturnsAsync(identityResult);

        // Act
        var result = await _identityService.ChangePasswordAsync(request);

        // Assert
        Assert.IsFalse(result.Success);
        Assert.IsTrue(result.Errors.Contains("Incorrect password"));
    }

    [TestMethod]
    public async Task ChangePasswordAsync_MissingFields_ReturnsFailure()
    {
        // Arrange
        var request = new ChangePasswordRequestDto("", "", "");

        // Act
        var result = await _identityService.ChangePasswordAsync(request);

        // Assert
        Assert.IsFalse(result.Success);
        Assert.IsTrue(result.Errors.Contains("All fields are required"));
    }
}
