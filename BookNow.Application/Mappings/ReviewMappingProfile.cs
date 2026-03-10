using AutoMapper;
using BookNow.Application.DTOs.Review;
using BookNow.Domain.Entities;

namespace BookNow.Application.Mappings;

public class ReviewMappingProfile : Profile
{
    public ReviewMappingProfile()
    {
        CreateMap<Review, ReviewDto>()
            .ConstructUsing(src => new ReviewDto(
                src.Id,
                src.ClientProfileId,
                "Client", // We'll need to join with UserProfile to get the name, but for now placeholder or use Identity
                src.Rating,
                src.Comment,
                src.CreatedAt
            ));
    }
}
