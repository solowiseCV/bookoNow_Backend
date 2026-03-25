namespace BookNow.Domain.Entities;
public class RevokedToken
{
    public Guid Id { get; set; }
    public string Jti { get; set; } = string.Empty; 
    public string UserId { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime RevokedAt { get; set; } = DateTime.UtcNow;
}