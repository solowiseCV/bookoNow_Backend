namespace BookNow.Domain.Entities
{
    public class Workshop : BaseEntity
    {
        public Guid MechanicProfileId { get; private set; }

        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Address { get; private set; }

        public double Latitude { get; private set; }
        public double Longitude { get; private set; }

        public bool IsVerified { get; private set; }

        public UserProfile MechanicProfile { get; private set; }
        public ICollection<Appointment> Appointments { get; private set; } = new List<Appointment>();
        public ICollection<Review> Reviews { get; private set; } = new List<Review>();

        protected Workshop() { }

        public Workshop(
            Guid mechanicProfileId,
            string name,
            string description,
            string address,
            double latitude,
            double longitude)
        {
            if (mechanicProfileId == Guid.Empty)
                throw new ArgumentException("Mechanic profile id is required.", nameof(mechanicProfileId));
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Workshop name cannot be empty.", nameof(name));
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Workshop description cannot be empty.", nameof(description));
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("Workshop address cannot be empty.", nameof(address));

            MechanicProfileId = mechanicProfileId;
            Name = name;
            Description = description;
            Address = address;
            Latitude = latitude;
            Longitude = longitude;
            IsVerified = false;
        }

        public void Verify()
        {
            if (IsVerified)
                return;

            IsVerified = true;
            SetUpdated();
        }
    }
}
