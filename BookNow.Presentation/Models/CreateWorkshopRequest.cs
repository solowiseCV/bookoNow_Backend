using BookNow.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace BookNow.Presentation.Models
{
    public class CreateWorkshopRequest
    {
        [Required(ErrorMessage = "Workshop name is required")]
        [MaxLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
        public string Name { get; set; } = default!;

        [Required(ErrorMessage = "Description is required")]
        [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string Description { get; set; } = default!;

        [Required(ErrorMessage = "Address is required")]
        [MaxLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
        public string Address { get; set; } = default!;

        [MaxLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string? PhoneNumber { get; set; }

        [MaxLength(500, ErrorMessage = "Opening hours cannot exceed 500 characters")]
        public string? OpeningHours { get; set; }

        [Required(ErrorMessage = "Workshop type is required")]
        public WorkshopType Type { get; set; }

        [Required(ErrorMessage = "A hero image is required")]
        public IFormFile HeroImage { get; set; } = default!;

        public IEnumerable<IFormFile>? GalleryImages { get; set; }
    }
}
