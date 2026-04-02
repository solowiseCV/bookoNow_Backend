namespace BookNow.Presentation.Models
{
    public class CreateAppointmentRequest
    {
        public Guid WorkshopId { get; set; }
        public DateTime AppointmentAt { get; set; }
        public string IssueDescription { get; set; } = default!;
        public List<IFormFile> MediaFiles { get; set; } = new();
    }
}