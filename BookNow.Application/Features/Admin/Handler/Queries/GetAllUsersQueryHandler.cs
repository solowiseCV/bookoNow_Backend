using BookNow.Application.Interfaces.Persistence;
using BookNow.Application.Features.Admin.Queries;
using MediatR;
using BookNow.Domain.Common;
using System.Collections.Generic;
using System.Linq;

namespace BookNow.Application.Features.Admin.Handler.Queries;

public class GetAllUsersQueryHandler(IUnitOfWork unitOfWork) 
    : IRequestHandler<GetAllUsersQuery, Result<IEnumerable<UserResponseDto>>>
{
    public async Task<Result<IEnumerable<UserResponseDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await unitOfWork.UserProfiles.GetAllAsync(cancellationToken);
        
        var response = users.Select(u => new UserResponseDto
        {
            Id = u.Id,
            FullName = u.FullName,
            Email = u.Email,
            Role = u.Role.ToString(),
            PhoneNumber = u.PhoneNumber,
            CreatedAt = u.CreatedAt
        });

        return Result<IEnumerable<UserResponseDto>>.Success(response);
    }
}
