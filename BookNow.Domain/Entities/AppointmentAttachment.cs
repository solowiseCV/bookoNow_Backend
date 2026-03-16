using System;

namespace BookNow.Domain.Entities
{
    public enum MediaType
    {
        Image,
        Video
    }

    public class AppointmentAttachment : BaseEntity
    {
        public Guid AppointmentId { get; private set; }
        public Appointment Appointment { get; private set; } = default!;

        public string Url { get; private set; } = default!;
        public MediaType Type { get; private set; }

        protected AppointmentAttachment() { }

        internal AppointmentAttachment(Appointment appointment, string url, MediaType type)
        {
            if (appointment == null) throw new ArgumentNullException(nameof(appointment));
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("Attachment URL cannot be empty.", nameof(url));

            Appointment = appointment;
            AppointmentId = appointment.Id;
            Url = url;
            Type = type;
        }
    }
}
