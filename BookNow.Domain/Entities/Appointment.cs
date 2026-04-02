using BookNow.Domain.Enums;

namespace BookNow.Domain.Entities
{
    public class Appointment : BaseEntity
    {
        public Guid ClientProfileId { get; private set; }
        public Guid WorkshopId { get; private set; }

        public DateTime AppointmentAt { get; private set; }
        public AppointmentStatus Status { get; private set; }

        public string IssueDescription { get; private set; } = default!;
        public string? RejectionReason { get; private set; }

        public Review? Review { get; private set; }
        public Conversation? Conversation { get; private set; }
        public ICollection<AppointmentAttachment> Attachments { get; private set; } = new List<AppointmentAttachment>();
 
        protected Appointment() { }

        public Appointment(
            Guid clientProfileId,
            Guid workshopId,
            DateTime appointmentAt,
            string issueDescription)
        {
            if (clientProfileId == Guid.Empty)
                throw new ArgumentException("Client profile id is required.", nameof(clientProfileId));
            if (workshopId == Guid.Empty)
                throw new ArgumentException("Workshop id is required.", nameof(workshopId));
            if (appointmentAt <= DateTime.UtcNow)
                throw new ArgumentException("Appointment time must be in the future.", nameof(appointmentAt));
            if (string.IsNullOrWhiteSpace(issueDescription))
                throw new ArgumentException("Issue description cannot be empty.", nameof(issueDescription));

            ClientProfileId = clientProfileId;
            WorkshopId = workshopId;
            AppointmentAt = appointmentAt;
            IssueDescription = issueDescription;
            Status = AppointmentStatus.Requested;
        }

        public void Accept()
        {
            if (Status != AppointmentStatus.Requested)
                throw new InvalidOperationException("Only requested appointments can be accepted.");

            Status = AppointmentStatus.Accepted;
            SetUpdated();
        }

        public void Reject(string reason)
        {
            if (Status != AppointmentStatus.Requested)
                throw new InvalidOperationException("Only requested appointments can be rejected.");

            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("Rejection reason cannot be empty.", nameof(reason));

            Status = AppointmentStatus.Rejected;
            RejectionReason = reason;
            SetUpdated();
        }

        public void Cancel()
        {
            if (Status == AppointmentStatus.Completed)
                throw new InvalidOperationException("Completed appointments cannot be cancelled.");

            Status = AppointmentStatus.Cancelled;
            SetUpdated();
        }

        public void UpdateAppointmentTime(DateTime newTime)
        {
            if (newTime <= DateTime.UtcNow)
                throw new ArgumentException("Appointment time must be in the future.", nameof(newTime));
            
            AppointmentAt = newTime;
            SetUpdated();
        }

        public void UpdateIssueDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Issue description cannot be empty.", nameof(description));
            
            IssueDescription = description;
            SetUpdated();
        }

        public void AddAttachment(string url, MediaType type)
        {
            var attachment = new AppointmentAttachment(this, url, type);
            Attachments.Add(attachment);
            SetUpdated();
        }
    }
}
