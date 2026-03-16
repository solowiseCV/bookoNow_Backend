using BookNow.Application.DTOs.Authentication.Response; 
using BookNow.Domain.Entities;
using BookNow.Domain.Enums;

namespace BookNow.Application.Interfaces.Authentication;

public interface IJwtTokenGenerator
{
    (string Token, DateTime ExpiresAt) GenerateToken(Guid userId, string email, string role);
}
