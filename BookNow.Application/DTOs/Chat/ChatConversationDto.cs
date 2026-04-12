namespace BookNow.Application.DTOs.Chat;

public class ChatConversationDto
{
    public Guid Id { get; set; }
    public Guid? AppointmentId { get; set; }
    public IReadOnlyList<Guid> ParticipantIds { get; set; } = Array.Empty<Guid>();
    public string? LastMessage { get; set; }
    public DateTime? LastMessageAt { get; set; }
}
