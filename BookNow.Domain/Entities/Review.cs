using BookNow.Domain.Enums;

namespace BookNow.Domain.Entities;

public class Review : BaseEntity
{
    public Guid ClientProfileId { get; private set; }
    public Guid WorkshopId { get; private set; }
    public Guid AppointmentId { get; private set; }

    public int Rating { get; private set; }
    public string Comment { get; private set; }

    public UserProfile ClientProfile { get; private set; }
    public Workshop Workshop { get; private set; }
    public Appointment Appointment { get; private set; }

    protected Review() { }

    public Review(
        Guid clientProfileId,
        Guid workshopId,
        Guid appointmentId,
        int rating,
        string comment)
    {
        if (rating < 1 || rating > 5)
            throw new ArgumentOutOfRangeException(nameof(rating), "Rating must be between 1 and 5.");

        if (string.IsNullOrWhiteSpace(comment))
            throw new ArgumentException("Review comment cannot be empty.", nameof(comment));

        ClientProfileId = clientProfileId;
        WorkshopId = workshopId;
        AppointmentId = appointmentId;
        Rating = rating;
        Comment = comment;
    }

    public void Update(int rating, string comment)
    {
        if (rating < 1 || rating > 5)
            throw new ArgumentOutOfRangeException(nameof(rating));

        if (string.IsNullOrWhiteSpace(comment))
            throw new ArgumentException(nameof(comment));

        Rating = rating;
        Comment = comment;
        UpdatedAt = DateTime.UtcNow;
    }
}
