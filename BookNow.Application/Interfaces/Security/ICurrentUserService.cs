
using BookNow.Domain.Enums;
namespace BookNow.Application.Interfaces.Security;
public interface ICurrentUserService
{
    Guid? IdentityUserId { get; }
    Guid? UserProfileId { get; }
    UserRole? Role { get; }
}

