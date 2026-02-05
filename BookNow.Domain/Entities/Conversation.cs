namespace BookNow.Domain.Entities;

public class Conversation : BaseEntity
{
    public Guid AppointmentId { get; private set; }

    public Appointment Appointment { get; private set; }
    public ICollection<Message> Messages { get; private set; } = new List<Message>();

    protected Conversation() { }

    public Conversation(Guid appointmentId)
    {
        AppointmentId = appointmentId;
    }
}
