using BookNow.Application.DTOs.Authentication.Request;
using BookNow.Application.Features.Authentication.Request.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BookNow.Presentation.Controllers;

[ApiController]
[Route("auth")]
public class AuthenticationController(ISender _sender) : ControllerBase
{

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequestDto request)
    {
        var command = new RegisterUserCommand(request);
        var result = await _sender.Send(command);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var command = new LoginUserCommand(request);
        var result = await _sender.Send(command);
        return result.Success ? Ok(result) : Unauthorized(result);
    }

    [HttpPost("google")]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleAuthRequestDto request)
    {
        var command = new GoogleAuthCommand(request);
        var result = await _sender.Send(command);

        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
    {
        var command = new ForgotPasswordCommand(request);
        var result = await _sender.Send(command);

        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request)
    {
        var command = new ResetPasswordCommand(request);
        var result = await _sender.Send(command);

        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
    {
        var command = new ChangePasswordCommand(request);
        var result = await _sender.Send(command);

        return result.Success ? Ok(result) : BadRequest(result);
    }

    [Authorize]
    [HttpPost("send-phone-verification")]
    public async Task<IActionResult> SendPhoneVerification([FromBody] SendPhoneVerificationRequestDto request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var command = new SendPhoneVerificationCommand(userId, request);
        var result = await _sender.Send(command);

        return result.Success ? Ok(result) : BadRequest(result);
    }

    [Authorize]
    [HttpPost("verify-phone")]
    public async Task<IActionResult> VerifyPhone([FromBody] VerifyPhoneRequestDto request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var command = new VerifyPhoneCommand(userId, request);
        var result = await _sender.Send(command);

        return result.Success ? Ok(result) : BadRequest(result);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var command = new LogoutUserCommand();
        var result = await _sender.Send(command);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
