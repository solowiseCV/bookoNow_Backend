using BookNow.Application.DTOs.Admin;
using BookNow.Domain.Common;
using MediatR;
using System.Collections.Generic;

namespace BookNow.Application.Features.Admin.Queries;

public record GetAllUsersQuery() : IRequest<Result<IEnumerable<UserResponseDto>>>;

public class UserResponseDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Role { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}
