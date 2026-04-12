namespace BookNow.Domain.Entities;

public class Conversation : BaseEntity
{
    public Guid? AppointmentId { get; private set; }

    public Appointment? Appointment { get; private set; }
    public ICollection<ConversationParticipant> Participants { get; private set; } = new List<ConversationParticipant>();
    public ICollection<Message> Messages { get; private set; } = new List<Message>();

    protected Conversation() { }

    public Conversation(Guid? appointmentId = null)
    {
        if (appointmentId.HasValue && appointmentId == Guid.Empty)
            throw new ArgumentException("Appointment id cannot be empty.", nameof(appointmentId));

        AppointmentId = appointmentId;
    }

    public void AddParticipant(Guid profileId)
    {
        if (profileId == Guid.Empty)
            throw new ArgumentException("Profile id is required.", nameof(profileId));

        if (Participants.Any(p => p.ProfileId == profileId))
            return;

        Participants.Add(new ConversationParticipant(Id, profileId));
        SetUpdated();
    }

    public void Touch()
    {
        SetUpdated();
    }
}
