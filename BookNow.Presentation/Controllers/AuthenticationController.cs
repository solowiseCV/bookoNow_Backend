using BookNow.Application.DTOs.Authentication.Request;
using BookNow.Application.Features.Authentication.Request.Commands;
using MediatR;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

using BookNow.Presentation.Models;

namespace BookNow.Presentation.Controllers;

[ApiController]
[Route("auth")]
public class AuthenticationController(ISender _sender) : BaseApiController
{

  
    [SwaggerOperation(Summary = "Registers a new user")]
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequestDto request)
    {
        var command = new RegisterUserCommand(request);
        var result = await _sender.Send(command);
        return result.Success ? Ok(new ApiResponse<object>(true, "User registered successfully", result)) : BadRequest(new ApiResponse(false, result.Message));
    }

  
    [SwaggerOperation(Summary = "Authenticates a user and returns a token")]
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var command = new LoginUserCommand(request);
        var result = await _sender.Send(command);
        return result.Success ? Ok(new ApiResponse<object>(true, "Login successful", result)) : Unauthorized(new ApiResponse(false, result.Message));
    }

  
    [SwaggerOperation(Summary = "Authenticates a user using Google OAuth")]
    [HttpPost("google")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleAuthRequestDto request)
    {
        var command = new GoogleAuthCommand(request);
        var result = await _sender.Send(command);

        return result.Success ? Ok(new ApiResponse<object>(true, "Google login successful", result)) : BadRequest(new ApiResponse(false, result.Message));
    }

    [SwaggerOperation(Summary = "Initiates the password recovery process")]
    [HttpPost("forgot-password")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
    {
        var command = new ForgotPasswordCommand(request);
        var result = await _sender.Send(command);

        return result.Success ? Ok(new ApiResponse<object>(true, "Password reset email sent", result)) : BadRequest(new ApiResponse(false, result.Message));
    }

  
    [SwaggerOperation(Summary = "Resets the user's password using a token")]
    [HttpPost("reset-password")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request)
    {
        var command = new ResetPasswordCommand(request);
        var result = await _sender.Send(command);

        return result.Success ? Ok(new ApiResponse<object>(true, "Password reset successful", result)) : BadRequest(new ApiResponse(false, result.Message));
    }

    [SwaggerOperation(Summary = "Changes the user's password")]
    [HttpPost("change-password")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
    {
        var command = new ChangePasswordCommand(request);
        var result = await _sender.Send(command);

        return result.Success ? Ok(new ApiResponse<object>(true, "Password changed successfully", result)) : BadRequest(new ApiResponse(false, result.Message));
    }

  
    [SwaggerOperation(Summary = "Sends a verification code to the user's phone")]
    [Authorize]
    [HttpPost("send-phone-verification")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SendPhoneVerification([FromBody] SendPhoneVerificationRequestDto request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized(new ApiResponse(false, "User ID not found in token."));

        var command = new SendPhoneVerificationCommand(userId, request);
        var result = await _sender.Send(command);

        return result.Success ? Ok(new ApiResponse<object>(true, "Verification code sent", result)) : BadRequest(new ApiResponse(false, result.Message));
    }

  
    [SwaggerOperation(Summary = "Verifies the user's phone number using a code")]
    [Authorize]
    [HttpPost("verify-phone")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VerifyPhone([FromBody] VerifyPhoneRequestDto request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized(new ApiResponse(false, "User ID not found in token."));

        var command = new VerifyPhoneCommand(userId, request);
        var result = await _sender.Send(command);

        return result.Success ? Ok(new ApiResponse<object>(true, "Phone number verified successfully", result)) : BadRequest(new ApiResponse(false, result.Message));
    }

 
    [SwaggerOperation(Summary = "Logs out the current user")]
    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Logout()
    {
        var command = new LogoutUserCommand();
        var result = await _sender.Send(command);
        return result.Success ? Ok(new ApiResponse<object>(true, "Logout successful", result)) : BadRequest(new ApiResponse(false, result.Message));
    }
}
