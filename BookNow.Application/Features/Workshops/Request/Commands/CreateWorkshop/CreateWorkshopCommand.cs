using MediatR;
using BookNow.Application.Models;
using BookNow.Domain.Enums;

namespace BookNow.Application.Features.Workshops.Request.Commands.CreateWorkshop;

public sealed record CreateWorkshopCommand(
    string Name,
    string Description,
    string Address,
    string PhoneNumber,
    string OpeningHours,
    WorkshopType Type,
    MediaFile? HeroImage,
    List<MediaFile>? GalleryImages
) : IRequest<Guid>;


