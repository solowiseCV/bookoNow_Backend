using BookNow.Application.DTOs.Authentication.Request;
using BookNow.Application.DTOs.Authentication.Response;
using BookNow.Application.Interfaces.Authentication;
using BookNow.Application.Interfaces.Persistence;
using BookNow.Application.Interfaces.Services;
using BookNow.Domain.Entities;
using BookNow.Domain.Enums;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BookNow.Infrastructure.Identity;

public class IdentityService(
    UserManager<ApplicationUser> userManager,
    IJwtTokenGenerator jwtTokenGenerator,
    IUnitOfWork unitOfWork,
    IEmailService emailService,
    ILogger<IdentityService> logger,
    IOptions<GoogleAuthOptions> googleOptions
) : IIdentityService
{
    public async Task<AuthResultDto> RegisterAsync(RegisterUserRequestDto request)
    {
        try
        {
            var userExists = await userManager.FindByEmailAsync(request.Email);
        if (userExists != null)
        {
            return new AuthResultDto(false, "User with this email already exists", new[] { "Email already taken" });
        }

        // Simple name split strategy
        var names = request.FullName.Split(' ', 2);
        var firstName = names.Length > 0 ? names[0] : "";
        var lastName = names.Length > 1 ? names[1] : "";

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = firstName,
            LastName = lastName,
            PhoneNumber = request.PhoneNumber
        };

        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            return new AuthResultDto(false, "Registration Failed", result.Errors.Select(e => e.Description));
        }

        // Create UserProfile
        var userProfile = new UserProfile(
            identityUserId: user.Id,
            role: request.Role
        );

        await unitOfWork.UserProfiles.AddAsync(userProfile, CancellationToken.None);
        await unitOfWork.SaveChangesAsync(CancellationToken.None);

        var token = jwtTokenGenerator.GenerateToken(user.Id, user.Email!, request.Role.ToString());

        var userSummary = new UserSummaryDto(
            userProfile.Id,
            $"{user.FirstName} {user.LastName}".Trim(),
            user.Email!,
            userProfile.Role.ToString()
        );

        return new AuthResultDto(true, "Registration Successful", null, token.Token, userSummary, token.ExpiresAt);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred during user registration for email: {Email}", request.Email);
            throw; // Re-throw to be caught by global exception handler
        }
    }

    public async Task<AuthResultDto> LoginAsync(LoginRequestDto request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null || !await userManager.CheckPasswordAsync(user, request.Password))
        {
            return new AuthResultDto(false, "Invalid credentials", new[] { "Invalid email or password" });
        }

        var userProfile = await unitOfWork.UserProfiles.GetByIdentityIdAsync(user.Id, CancellationToken.None);

        if (userProfile == null)
            return new AuthResultDto(false, "Profile missing", new[] { "User profile not found" });

        var token = jwtTokenGenerator.GenerateToken(user.Id, user.Email!, userProfile.Role.ToString());

        var userSummary = new UserSummaryDto(
            userProfile.Id,
            $"{user.FirstName} {user.LastName}".Trim(),
            user.Email!,
            userProfile.Role.ToString()
        );

        return new AuthResultDto(true, "Login Successful", null, token.Token, userSummary, token.ExpiresAt);
    }

   public async Task<AuthResultDto> LoginWithGoogleAsync(
    GoogleAuthRequestDto request,
    CancellationToken ct = default)
  {
    try
    {
        if (string.IsNullOrWhiteSpace(request.IdToken))
            return new AuthResultDto(false, "Google Auth Failed", new[] { "IdToken is required" });

        var settings = new GoogleJsonWebSignature.ValidationSettings
        {
            Audience = new[] { googleOptions.Value.ClientId } 
        };

        var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, settings);

        if (!payload.EmailVerified)
            return new AuthResultDto(false, "Google email not verified", new[] { "Email is not verified by Google" });

        var user = await userManager.FindByEmailAsync(payload.Email);

        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = payload.Email,
                Email = payload.Email,
                FirstName = payload.GivenName,
                LastName = payload.FamilyName
            };

            var createResult = await userManager.CreateAsync(user);

            if (!createResult.Succeeded)
                return new AuthResultDto(
                    false,
                    "Failed to create Google user",
                    createResult.Errors.Select(e => e.Description));
        }

        // Ensure profile exists
        var profile = await unitOfWork.UserProfiles.GetByIdentityIdAsync(user.Id, ct);

        if (profile == null)
        {
            profile = new UserProfile(user.Id, UserRole.Client);
            await unitOfWork.UserProfiles.AddAsync(profile, ct);
            await unitOfWork.SaveChangesAsync(ct);
        }

        var token = jwtTokenGenerator.GenerateToken(
            user.Id,
            user.Email!,
            profile.Role.ToString());

        var userSummary = new UserSummaryDto(
            profile.Id,
            $"{user.FirstName} {user.LastName}".Trim(),
            user.Email!,
            profile.Role.ToString()
        );

        return new AuthResultDto(true, "Google Login Successful", null, token.Token, userSummary, token.ExpiresAt);
    }
    catch (InvalidJwtException ex)
    {
        logger.LogWarning(ex, "Invalid Google JWT token provided for authentication");
        return new AuthResultDto(false, "Google Auth Failed", new[] { "Invalid Google token" });
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Unexpected error during Google authentication");
        return new AuthResultDto(false, "Google Auth Failed", new[] { "Something went wrong" });
    }
}

    public async Task<AuthResultDto> ForgotPasswordAsync(ForgotPasswordRequestDto request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return new AuthResultDto(false, "Forgot password failed", new[] { "Email is required" });
            }

            var user = await userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                logger.LogInformation("Password reset requested for non-existent email: {Email}", request.Email);
                return new AuthResultDto(true, "If that email is registered, a password reset link has been sent.", null);
            }

            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            await emailService.SendPasswordResetEmailAsync(user.Email!, token);

            logger.LogInformation("Password reset email sent successfully to {Email}", request.Email);
            return new AuthResultDto(true, "Password reset link sent successfully.", null);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred during password reset request for email: {Email}", request.Email);
            return new AuthResultDto(false, "Forgot password failed", new[] { "An error occurred. Please try again later." });
        }
    }

    public async Task<AuthResultDto> ResetPasswordAsync(ResetPasswordRequestDto request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Token) ||
                string.IsNullOrWhiteSpace(request.NewPassword))
            {
                return new AuthResultDto(false, "Password reset failed", new[] { "All fields are required" });
            }

            var user = await userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                logger.LogWarning("Password reset attempted for non-existent email: {Email}", request.Email);
                return new AuthResultDto(false, "Password reset failed", new[] { "Invalid request" });
            }

            var result = await userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);

            if (!result.Succeeded)
            {
                logger.LogWarning("Password reset failed for email {Email}: {Errors}",
                    request.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
                return new AuthResultDto(false, "Password reset failed", result.Errors.Select(e => e.Description));
            }

            logger.LogInformation("Password reset successful for email {Email}", request.Email);
            return new AuthResultDto(true, "Password has been reset successfully.", null);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred during password reset for email: {Email}", request.Email);
            return new AuthResultDto(false, "Password reset failed", new[] { "An error occurred. Please try again later." });
        }
    }

    public async Task<AuthResultDto> ChangePasswordAsync(ChangePasswordRequestDto request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.UserId) ||
                string.IsNullOrWhiteSpace(request.CurrentPassword) ||
                string.IsNullOrWhiteSpace(request.NewPassword))
            {
                return new AuthResultDto(false, "Password change failed", new[] { "All fields are required" });
            }

            var user = await userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                logger.LogWarning("Password change attempted for non-existent user: {UserId}", request.UserId);
                return new AuthResultDto(false, "Password change failed", new[] { "User not found" });
            }

            var result = await userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

            if (!result.Succeeded)
            {
                logger.LogWarning("Password change failed for user {UserId}: {Errors}", request.UserId, string.Join(", ", result.Errors.Select(e => e.Description)));
                return new AuthResultDto(false, "Password change failed", result.Errors.Select(e => e.Description));
            }

            logger.LogInformation("Password changed successfully for user {UserId}", request.UserId);
            return new AuthResultDto(true, "Password changed successfully.", null);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during password change for user: {UserId}", request.UserId);
            return new AuthResultDto(false, "Password change failed", new[] { "An error occurred. Please try again later." });
        }
    }

    public async Task<AuthResultDto> GetProfileAsync(string userId, CancellationToken ct = default)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return new AuthResultDto(false, "User not found", new[] { "User not found" });
        }

        var userProfile = await unitOfWork.UserProfiles.GetByIdentityIdAsync(Guid.Parse(userId), ct);
        if (userProfile == null)
        {
            return new AuthResultDto(false, "Profile not found", new[] { "User profile not found" });
        }

        return new AuthResultDto(true, "Profile retrieved successfully", null);
    }

    public async Task<AuthResultDto> UpdateProfileAsync(string userId, UpdateProfileRequestDto request, CancellationToken ct = default)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return new AuthResultDto(false, "User not found", new[] { "User not found" });
        }

        if (!string.IsNullOrWhiteSpace(request.FullName))
        {
            var names = request.FullName.Split(' ', 2);
            user.FirstName = names.Length > 0 ? names[0] : user.FirstName;
            user.LastName = names.Length > 1 ? names[1] : user.LastName;
        }
        if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
            user.PhoneNumber = request.PhoneNumber;

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            return new AuthResultDto(false, "Profile update failed", result.Errors.Select(e => e.Description));
        }

        return new AuthResultDto(true, "Profile updated successfully", null);
    }

    public async Task<AuthResultDto> SendPhoneVerificationAsync(string userId, SendPhoneVerificationRequestDto request)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return new AuthResultDto(false, "User not found", new[] { "User not found" });
        }

        // TODO: Implement actual phone verification logic (e.g., send SMS code)
        return new AuthResultDto(true, "Verification code sent", null);
    }

    public async Task<AuthResultDto> VerifyPhoneAsync(string userId, VerifyPhoneRequestDto request)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return new AuthResultDto(false, "User not found", new[] { "User not found" });
        }

        // TODO: Implement actual phone verification logic
        return new AuthResultDto(true, "Phone verified successfully", null);
    }

    public Task<AuthResultDto> LogoutAsync()
    {
        // TODO: Implement actual logout logic (e.g., invalidate tokens)
        return Task.FromResult(new AuthResultDto(true, "Logged out successfully", null));
    }
}
