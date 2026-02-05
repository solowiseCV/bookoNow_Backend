using BookNow.Domain.Enums;

namespace BookNow.Domain.Entities
{
    public class Appointment : BaseEntity
    {
        public Guid ClientProfileId { get; private set; }
        public Guid WorkshopId { get; private set; }

        public DateTime AppointmentAt { get; private set; }
        public AppointmentStatus Status { get; private set; }

        public string IssueDescription { get; private set; }

        public Review Review { get; private set; }
        public Conversation Conversation { get; private set; }
 
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

        public void Reject()
        {
            if (Status != AppointmentStatus.Requested)
                throw new InvalidOperationException("Only requested appointments can be rejected.");

            Status = AppointmentStatus.Rejected;
            SetUpdated();
        }

        public void Cancel()
        {
            if (Status == AppointmentStatus.Completed)
                throw new InvalidOperationException("Completed appointments cannot be cancelled.");

            Status = AppointmentStatus.Cancelled;
            SetUpdated();
        }
    }
}
