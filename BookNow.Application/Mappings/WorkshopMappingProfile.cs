
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
            .ForCtorParam("ReviewCount", opt => opt.MapFrom(s => s.Reviews != null ? s.Reviews.Count : 0))
            .ForCtorParam("GalleryImages", opt => opt.MapFrom(s => s.GalleryImages.Select(i => i.Url).ToList()))
            .ForCtorParam("Reviews", opt => opt.MapFrom(s => s.Reviews.Select(r => r.Comment).ToList()))
            .ForCtorParam("OwnerName", opt => opt.MapFrom(s => s.MechanicProfile != null ? s.MechanicProfile.FullName : null))
            .ForCtorParam("OwnerEmail", opt => opt.MapFrom(s => s.MechanicProfile != null ? s.MechanicProfile.Email : null));
    }
}

