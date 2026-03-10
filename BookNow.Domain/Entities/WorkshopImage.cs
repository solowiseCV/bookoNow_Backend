namespace BookNow.Domain.Entities
{
    public class WorkshopImage : BaseEntity
    {
        public Guid WorkshopId { get; private set; }
        public string Url { get; private set; }
        public string? PublicId { get; private set; }

        public Workshop Workshop { get; private set; }

        protected WorkshopImage() { }

        public WorkshopImage(Guid workshopId, string url, string? publicId = null)
        {
            if (workshopId == Guid.Empty)
                throw new ArgumentException("Workshop id is required.", nameof(workshopId));
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("Image url cannot be empty.", nameof(url));

            WorkshopId = workshopId;
            Url = url;
            PublicId = publicId;
        }
    }
}
