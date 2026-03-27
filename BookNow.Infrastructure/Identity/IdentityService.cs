using System.IdentityModel.Tokens.Jwt;
using System.Text;
using BookNow.Application.DTOs.Authentication.Request;
using BookNow.Application.DTOs.Authentication.Response;
using BookNow.Application.Interfaces.Authentication;
using BookNow.Application.Interfaces.Persistence;
using BookNow.Application.Interfaces.Services;
using BookNow.Domain.Entities;
using BookNow.Domain.Enums;
using BookNow.Infrastructure.Data;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Verify.V2.Service;
namespace BookNow.Infrastructure.Identity;

public class IdentityService(
    UserManager<ApplicationUser> userManager,
    IJwtTokenGenerator jwtTokenGenerator,
    IUnitOfWork unitOfWork,
    BookNowDbContext dbContext,
    IEmailService emailService,
    ILogger<IdentityService> logger,
    IConfiguration configuration,
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
                userProfile.Role.ToString(),
                user.PhoneNumber ?? "",
                false
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
            userProfile.Role.ToString(),
            user.PhoneNumber ?? "",
            userProfile.Workshops.Any()
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
                return new AuthResultDto(false, "Google email not verified",
                    new[] { "Email is not verified by Google" });

            if (string.IsNullOrWhiteSpace(payload.Email))
                return new AuthResultDto(false, "Google Auth Failed",
                    new[] { "Email not provided by Google" });

            var email = payload.Email.ToLowerInvariant();

            logger.LogInformation("Google login attempt for {Email}", email);

            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    FirstName = payload.GivenName ?? "",
                    LastName = payload.FamilyName ?? ""
                };

                var createResult = await userManager.CreateAsync(user);

                if (!createResult.Succeeded)
                    return new AuthResultDto(
                        false,
                        "Failed to create Google user",
                        createResult.Errors.Select(e => e.Description));
            }

            // Link Google login properly
            var loginInfo = new UserLoginInfo("Google", payload.Subject, "Google");
            var logins = await userManager.GetLoginsAsync(user);

            if (!logins.Any(l => l.LoginProvider == loginInfo.LoginProvider &&
                                 l.ProviderKey == loginInfo.ProviderKey))
            {
                await userManager.AddLoginAsync(user, loginInfo);
            }

            // Ensure profile exists
            var profile = await unitOfWork.UserProfiles.GetByIdentityIdAsync(user.Id, ct);

            if (profile == null)
            {
                profile = new UserProfile(user.Id, UserRole.Client);
                await unitOfWork.UserProfiles.AddAsync(profile, ct);
                await unitOfWork.SaveChangesAsync(ct);
                // Reload with workshops (will be empty)
                profile = await unitOfWork.UserProfiles.GetByIdentityIdAsync(user.Id, ct);
            }

            var token = jwtTokenGenerator.GenerateToken(
                user.Id,
                user.Email!,
                profile!.Role.ToString());

            var userSummary = new UserSummaryDto(
                profile.Id,
                $"{user.FirstName} {user.LastName}".Trim(),
                user.Email!,
                profile.Role.ToString(),
                user.PhoneNumber ?? "",
                profile.Workshops.Any()
            );

            return new AuthResultDto(
                true,
                "Google Login Successful",
                null,
                token.Token,
                userSummary,
                token.ExpiresAt);
        }
        catch (InvalidJwtException ex)
        {
            logger.LogWarning(ex, "Invalid Google JWT token");
            return new AuthResultDto(false, "Google Auth Failed", new[] { "Invalid Google token" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected Google auth error");
            return new AuthResultDto(false, "Google Auth Failed", new[] { "Something went wrong" });
        }
    }
    public async Task<AuthResultDto> ForgotPasswordAsync(ForgotPasswordRequestDto request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        // Always return success for security
        if (user == null)
            return new AuthResultDto(true, "If the email exists, a reset link has been sent.", null);

        // Generate token
        var token = await userManager.GeneratePasswordResetTokenAsync(user);

        // Encode token for URL safely
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        // Build reset link
        var clientUrl = configuration["FrontendSettings:Url"] ?? "https://booknow-three.vercel.app";
        var resetLink = $"{clientUrl.TrimEnd('/')}/auth/reset-password?email={Uri.EscapeDataString(user.Email!)}&token={Uri.EscapeDataString(encodedToken)}";

        // Log for debug (remove in production)
        logger.LogInformation("Reset link: {Link}", resetLink);

        // Send email
        await emailService.SendPasswordResetEmailAsync(user.Email, resetLink);

        return new AuthResultDto(true, "Password reset email sent", null);
    }
    public async Task<AuthResultDto> ResetPasswordAsync(ResetPasswordRequestDto request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Token) ||
                string.IsNullOrWhiteSpace(request.NewPassword))
            {
                return new AuthResultDto(false, "Password reset failed",
                    new[] { "All fields are required" });
            }

            var user = await userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                logger.LogWarning("Password reset attempted for non-existent email: {Email}", request.Email);
                return new AuthResultDto(false, "Password reset failed",
                    new[] { "Invalid request" });
            }

            // Decode token safely
            var decodedTokenBytes = WebEncoders.Base64UrlDecode(request.Token);
            var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);

            var result = await userManager.ResetPasswordAsync(user, decodedToken, request.NewPassword);

            if (!result.Succeeded)
            {
                logger.LogWarning("Password reset failed for email {Email}: {Errors}",
                    request.Email,
                    string.Join(", ", result.Errors.Select(e => e.Description)));

                return new AuthResultDto(false, "Password reset failed",
                    result.Errors.Select(e => e.Description));
            }

            logger.LogInformation("Password reset successful for email {Email}", request.Email);

            return new AuthResultDto(true, "Password has been reset successfully.", null);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred during password reset for email: {Email}", request.Email);

            return new AuthResultDto(false, "Password reset failed",
                new[] { "An error occurred. Please try again later." });
        }
    }
    public async Task<AuthResultDto> ChangePasswordAsync(string userId, ChangePasswordRequestDto request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.CurrentPassword) || string.IsNullOrWhiteSpace(request.NewPassword))
            {
                return new AuthResultDto(false, "Password change failed", new[] { "Current and new passwords are required" });
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                logger.LogWarning("Password change attempted for non-existent user: {UserId}", userId);
                return new AuthResultDto(false, "Password change failed", new[] { "User not found" });
            }

            var result = await userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToArray();
                logger.LogWarning("Password change failed for user {UserId}: {Errors}", userId, string.Join(", ", errors));
                return new AuthResultDto(false, "Password change failed", errors);
            }

            logger.LogInformation("Password changed successfully for user {UserId}", userId);
            return new AuthResultDto(true, "Password changed successfully", null);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during password change for user: {UserId}", userId);
            return new AuthResultDto(false, "Password change failed", new[] { "An unexpected error occurred. Please try again later." });
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

        var userSummary = new UserSummaryDto(
            userProfile.Id,
            $"{user.FirstName} {user.LastName}".Trim(),
            user.Email!,
            userProfile.Role.ToString(),
            user.PhoneNumber ?? "",
            userProfile.Workshops.Any()
        );

        return new AuthResultDto(true, "Profile retrieved successfully", null, null, userSummary);
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

        var userProfile = await unitOfWork.UserProfiles.GetByIdentityIdAsync(Guid.Parse(userId), ct);
        var userSummary = new UserSummaryDto(
            userProfile!.Id,
            $"{user.FirstName} {user.LastName}".Trim(),
            user.Email!,
            userProfile.Role.ToString(),
            user.PhoneNumber ?? "",
            userProfile.Workshops.Any()
        );

        return new AuthResultDto(true, "Profile updated successfully", null, null, userSummary);
    }



    public async Task<AuthResultDto> SendPhoneVerificationAsync(
        string userId, SendPhoneVerificationRequestDto request)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            return new AuthResultDto(false, "User not found", new[] { "User not found" });

        if (string.IsNullOrWhiteSpace(request.PhoneNumber))
            return new AuthResultDto(false, "Phone number is required", new[] { "Phone number is required" });

        // Save phone number to user
        user.PhoneNumber = request.PhoneNumber;
        await userManager.UpdateAsync(user);

        // Init Twilio
        TwilioClient.Init(
            configuration["Twilio:AccountSid"],
            configuration["Twilio:AuthToken"]
        );

        // Send OTP via Twilio Verify
        await VerificationResource.CreateAsync(
            to: request.PhoneNumber,
            channel: "sms",
            pathServiceSid: configuration["Twilio:VerifyServiceSid"]
        );

        return new AuthResultDto(true, "Verification code sent successfully", null);
    }

    public async Task<AuthResultDto> VerifyPhoneAsync(
        string userId, VerifyPhoneRequestDto request)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            return new AuthResultDto(false, "User not found", new[] { "User not found" });

        if (string.IsNullOrWhiteSpace(user.PhoneNumber))
            return new AuthResultDto(false, "No phone number found. Please request a new code.",
                new[] { "Phone number missing" });

        // Init Twilio
        TwilioClient.Init(
            configuration["Twilio:AccountSid"],
            configuration["Twilio:AuthToken"]
        );

        // Check the OTP code
        var verificationCheck = await VerificationCheckResource.CreateAsync(
            to: user.PhoneNumber,
            code: request.Code,
            pathServiceSid: configuration["Twilio:VerifyServiceSid"]
        );

        if (verificationCheck.Status != "approved")
            return new AuthResultDto(false, "Invalid or expired verification code",
                new[] { "Invalid or expired code" });

        // Mark phone as confirmed
        user.PhoneNumberConfirmed = true;
        await userManager.UpdateAsync(user);

        return new AuthResultDto(true, "Phone number verified successfully", null);
    }
    public async Task<AuthResultDto> LogoutAsync(string userId, string token)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            return new AuthResultDto(false, "User not found", new[] { "User not found" });

        // Extract JTI and expiry from token
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        var jti = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
        if (string.IsNullOrEmpty(jti))
            return new AuthResultDto(false, "Invalid token", new[] { "Invalid token" });

        // Blacklist the token
        var revokedToken = new RevokedToken
        {
            Id = Guid.NewGuid(),
            Jti = jti,
            UserId = userId,
            ExpiresAt = jwtToken.ValidTo,
            RevokedAt = DateTime.UtcNow
        };

        await dbContext.RevokedTokens.AddAsync(revokedToken);
        await dbContext.SaveChangesAsync();

        return new AuthResultDto(true, "Logged out successfully", null);
    }
}
