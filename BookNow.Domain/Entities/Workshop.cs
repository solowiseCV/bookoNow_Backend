using BookNow.Domain.Enums;

namespace BookNow.Domain.Entities
{
    public class Workshop : BaseEntity
    {
        public Guid MechanicProfileId { get; private set; }

        public string Name { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public string Address { get; private set; } = string.Empty;

        public string? PhoneNumber { get; private set; }
        public string? OpeningHours { get; private set; }
        public string? HeroImageUrl { get; private set; }

        public WorkshopType Type { get; private set; }

        public double Latitude { get; private set; }
        public double Longitude { get; private set; }

        public bool IsVerified { get; private set; }
        public bool IsSubscribed { get; private set; } = false;

        public string? PaystackSubaccountCode { get; private set; }

        public string? BankName { get; private set; }
        public string? BankCode { get; private set; }
        public string? AccountNumber { get; private set; }
        public string? AccountName { get; private set; }

        public UserProfile MechanicProfile { get; private set; } = null!;


        public List<Appointment> Appointments { get; private set; } = [];
        public List<Review> Reviews { get; private set; } = [];
        public List<WorkshopImage> GalleryImages { get; private set; } = [];
        protected Workshop() { }

        public Workshop(
            Guid mechanicProfileId,
            string name,
            string description,
            string address,
            double latitude,
            double longitude,
            WorkshopType type,
            string? heroImageUrl = null,
            string? phoneNumber = null,
            string? openingHours = null)
        {
            if (mechanicProfileId == Guid.Empty)
                throw new ArgumentException("Mechanic profile id is required.", nameof(mechanicProfileId));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Workshop name cannot be empty.", nameof(name));

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Workshop description cannot be empty.", nameof(description));

            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("Workshop address cannot be empty.", nameof(address));

            if (latitude < -90 || latitude > 90)
                throw new ArgumentException("Latitude must be between -90 and 90.", nameof(latitude));

            if (longitude < -180 || longitude > 180)
                throw new ArgumentException("Longitude must be between -180 and 180.", nameof(longitude));

            MechanicProfileId = mechanicProfileId;
            Name = name;
            Description = description;
            Address = address;

            Latitude = latitude;
            Longitude = longitude;

            PhoneNumber = phoneNumber;
            OpeningHours = openingHours;
            HeroImageUrl = heroImageUrl;

            Type = type;
            IsVerified = false;
        }

        public void Verify()
        {
            if (IsVerified)
                return;

            IsVerified = true;
            SetUpdated();
        }

        public void UpdateDetails(
            string? name,
            string? description,
            string? address,
            double latitude,
            double longitude,
            string? phoneNumber,
            string? openingHours,
            WorkshopType type,
            string? heroImageUrl)
        {
            if (!string.IsNullOrWhiteSpace(name))
                Name = name;

            if (!string.IsNullOrWhiteSpace(description))
                Description = description;

            if (!string.IsNullOrWhiteSpace(address))
                Address = address;

            if (latitude >= -90 && latitude <= 90)
                Latitude = latitude;

            if (longitude >= -180 && longitude <= 180)
                Longitude = longitude;

            PhoneNumber = phoneNumber ?? PhoneNumber;
            OpeningHours = openingHours ?? OpeningHours;
            HeroImageUrl = heroImageUrl ?? HeroImageUrl;

            Type = type;

            SetUpdated();
        }

        public void SetSubscription(bool subscribed)
        {
            IsSubscribed = subscribed;
            SetUpdated();
        }

        public void SetBankDetails(
            string bankName,
            string bankCode,
            string accountNumber,
            string accountName)
        {
            BankName = bankName;
            BankCode = bankCode;
            AccountNumber = accountNumber;
            AccountName = accountName;

            SetUpdated();
        }

        public void SetPaystackSubaccount(string subaccountCode)
        {
            PaystackSubaccountCode = subaccountCode;
            SetUpdated();
        }
    }
}

