using BookNow.Application.DTOs.Authentication.Request;
using BookNow.Application.DTOs.Authentication.Response;
using BookNow.Application.Features.Authentication.Handler.Commands;
using BookNow.Application.Features.Authentication.Request.Commands;
using BookNow.Application.Interfaces.Authentication;
using Moq;
using Xunit;

namespace BookNow.Test.Features.Authentication;

public class GoogleAuthCommandHandlerTests
{
    private readonly Mock<IIdentityService> _mockIdentityService;
    private readonly GoogleAuthCommandHandler _handler;

    public GoogleAuthCommandHandlerTests()
    {
        _mockIdentityService = new Mock<IIdentityService>();
        _handler = new GoogleAuthCommandHandler(_mockIdentityService.Object);
    }

    [Fact]
    public async Task Handle_ValidGoogleToken_ReturnsSuccessWithToken()
    {
        // Arrange
        var idToken = "valid-google-id-token";
        var googleRequest = new GoogleAuthRequestDto(idToken);
        var command = new GoogleAuthCommand(googleRequest);
        
        var expectedResult = new AuthResultDto(
            Success: true,
            Message: "Google Login Successful",
            Errors: null!,
            Token: "jwt-token-from-system"
        );

        _mockIdentityService
            .Setup(x => x.LoginWithGoogleAsync(It.IsAny<GoogleAuthRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Google Login Successful", result.Message);
        Assert.Equal("jwt-token-from-system", result.Token);
        _mockIdentityService.Verify(
            x => x.LoginWithGoogleAsync(It.Is<GoogleAuthRequestDto>(r => r.IdToken == idToken), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
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
            Token: null!
        );

        _mockIdentityService
            .Setup(x => x.LoginWithGoogleAsync(It.IsAny<GoogleAuthRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Google Auth Failed", result.Message);
        Assert.Null(result.Token);
    }

    [Fact]
    public async Task Handle_EmptyIdToken_ReturnsFailure()
    {
        // Arrange
        var googleRequest = new GoogleAuthRequestDto("");
        var command = new GoogleAuthCommand(googleRequest);

        var expectedResult = new AuthResultDto(
            Success: false,
            Message: "Google Auth Failed",
            Errors: new[] { "ID Token is required" },
            Token: null!
        );

        _mockIdentityService
            .Setup(x => x.LoginWithGoogleAsync(It.IsAny<GoogleAuthRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Null(result.Token);
    }

    [Fact]
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
            Token: null!
        );

        _mockIdentityService
            .Setup(x => x.LoginWithGoogleAsync(It.IsAny<GoogleAuthRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Null(result.Token);
    }
}
