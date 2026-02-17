using BookNow.Application.DTOs.Authentication.Request;
using BookNow.Application.DTOs.Authentication.Response;
using BookNow.Application.Features.Authentication.Handler.Commands;
using BookNow.Application.Features.Authentication.Request.Commands;
using BookNow.Application.Interfaces.Authentication;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace BookNow.Test.Features.Authentication;

[TestClass]
public class GoogleAuthCommandHandlerTests
{
    private Mock<IIdentityService>? _mockIdentityService;
    private GoogleAuthCommandHandler? _handler;

    [TestInitialize]
    public void Setup()
    {
        _mockIdentityService = new Mock<IIdentityService>();
        _handler = new GoogleAuthCommandHandler(_mockIdentityService.Object);
    }

    [TestMethod]
    public async Task Handle_ValidGoogleToken_ReturnsSuccessWithToken()
    {
        // Arrange
        var idToken = "valid-google-id-token";
        var googleRequest = new GoogleAuthRequestDto(idToken);
        var command = new GoogleAuthCommand(googleRequest);
        
        var expectedResult = new AuthResultDto(
            Success: true,
            Message: "Google Login Successful",
            Errors: null,
            Token: "jwt-token-from-system"
        );

        _mockIdentityService
            .Setup(x => x.LoginWithGoogleAsync(It.IsAny<GoogleAuthRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.Success);
        Assert.AreEqual("Google Login Successful", result.Message);
        Assert.AreEqual("jwt-token-from-system", result.Token);
        _mockIdentityService.Verify(
            x => x.LoginWithGoogleAsync(It.Is<GoogleAuthRequestDto>(r => r.IdToken == idToken), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [TestMethod]
    public async Task Handle_InvalidGoogleToken_ReturnsFailure()
    {
        // Arrange
        var idToken = "invalid-google-token";
        var googleRequest = new GoogleAuthRequestDto(idToken);
        var command = new GoogleAuthCommand(googleRequest);

        var expectedResult = new AuthResultDto(
            Success: false,
            Message: "Google Auth Failed",
            Errors: new[] { "Invalid token signature" },
            Token: null
        );

        _mockIdentityService
            .Setup(x => x.LoginWithGoogleAsync(It.IsAny<GoogleAuthRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsFalse(result.Success);
        Assert.AreEqual("Google Auth Failed", result.Message);
        Assert.IsNull(result.Token);
    }

    [TestMethod]
    public async Task Handle_EmptyIdToken_ReturnsFailure()
    {
        // Arrange
        var googleRequest = new GoogleAuthRequestDto("");
        var command = new GoogleAuthCommand(googleRequest);

        var expectedResult = new AuthResultDto(
            Success: false,
            Message: "Google Auth Failed",
            Errors: new[] { "ID Token is required" },
            Token: null
        );

        _mockIdentityService
            .Setup(x => x.LoginWithGoogleAsync(It.IsAny<GoogleAuthRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsFalse(result.Success);
        Assert.IsNull(result.Token);
    }

    [TestMethod]
    public async Task Handle_ServiceThrowsException_ReturnsFailure()
    {
        // Arrange
        var idToken = "any-token";
        var googleRequest = new GoogleAuthRequestDto(idToken);
        var command = new GoogleAuthCommand(googleRequest);

        var expectedResult = new AuthResultDto(
            Success: false,
            Message: "Google Auth Failed",
            Errors: new[] { "Unexpected error occurred" },
            Token: null
        );

        _mockIdentityService
            .Setup(x => x.LoginWithGoogleAsync(It.IsAny<GoogleAuthRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsFalse(result.Success);
        Assert.IsNull(result.Token);
    }
}
