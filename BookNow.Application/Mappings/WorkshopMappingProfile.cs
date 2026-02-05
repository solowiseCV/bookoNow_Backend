
using AutoMapper;
using BookNow.Application.DTOs.Workshop;
using BookNow.Domain.Entities;

namespace BookNow.Application.Mappings;
public sealed class WorkshopMappingProfile : Profile
{
    public WorkshopMappingProfile()
    {
        CreateMap<Workshop, WorkshopDto>();
    }
}

