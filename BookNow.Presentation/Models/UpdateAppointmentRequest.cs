using System;

namespace BookNow.Presentation.Models
{
    public class UpdateAppointmentRequest
    {
        public DateTime AppointmentAt { get; set; }
        public string IssueDescription { get; set; }
    }
}