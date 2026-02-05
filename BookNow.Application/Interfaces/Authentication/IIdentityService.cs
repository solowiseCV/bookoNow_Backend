

namespace BookNow.Application.Interfaces.Authentication
{
    public interface IIdentityService
    {
        Task<Guid> CreateUserAsync( string email,string password,string role,CancellationToken ct);
        Task<bool> ValidateUserAsync(string email, string password, CancellationToken ct);
        Task<Guid?> GetUserIdByEmailAsync(string email, CancellationToken ct);
        Task<string?> GetUserRoleAsync(Guid userId, CancellationToken ct);


    }

}
