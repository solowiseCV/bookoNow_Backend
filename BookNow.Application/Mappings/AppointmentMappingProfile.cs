using AutoMapper;
using BookNow.Application.DTOs.Appointment;
using BookNow.Domain.Entities;

namespace BookNow.Application.Mappings;

public sealed class AppointmentMappingProfile : Profile
{
    public AppointmentMappingProfile()
    {
        CreateMap<AppointmentAttachment, AppointmentAttachmentDto>();
        CreateMap<Appointment, AppointmentDto>()
            .ForMember(dest => dest.Attachments, opt => opt.MapFrom(src => src.Attachments));
    }
}
