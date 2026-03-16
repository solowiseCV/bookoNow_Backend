using BookNow.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace BookNow.Presentation.Models;

public class PatchWorkshopRequest
{
    [MaxLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
    public string? Name { get; set; }

    [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }

    [MaxLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
    public string? Address { get; set; }

    [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
    public double? Latitude { get; set; }

    [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
    public double? Longitude { get; set; }

    [MaxLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
    [Phone(ErrorMessage = "Invalid phone number format")]
    public string? PhoneNumber { get; set; }

    [MaxLength(500, ErrorMessage = "Opening hours cannot exceed 500 characters")]
    public string? OpeningHours { get; set; }

    public WorkshopType? Type { get; set; }

    public string? HeroImageUrl { get; set; }
}
