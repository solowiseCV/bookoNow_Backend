
using AutoMapper;
using BookNow.Application.DTOs.Workshop;
using BookNow.Domain.Entities;

namespace BookNow.Application.Mappings;
public sealed class WorkshopMappingProfile : Profile
{
    public WorkshopMappingProfile()
    {
        CreateMap<Workshop, WorkshopDto>()
            .ForCtorParam("AverageRating", opt => opt.MapFrom(s => (s.Reviews != null && s.Reviews.Any()) ? s.Reviews.Average(r => r.Rating) : 0))
            .ForCtorParam("ReviewCount", opt => opt.MapFrom(s => s.Reviews != null ? s.Reviews.Count : 0));
    }
}

