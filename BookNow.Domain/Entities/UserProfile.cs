using BookNow.Domain.Enums;

namespace BookNow.Domain.Entities
{
    public class UserProfile : BaseEntity
    {
        public Guid IdentityUserId { get; private set; }
        public UserRole Role { get; private set; }
        public string FullName { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string PhoneNumber { get; private set; } = string.Empty;

        public ICollection<Workshop> Workshops { get; private set; } = new List<Workshop>();
        public ICollection<Appointment> Appointments { get; private set; } = new List<Appointment>();
        public ICollection<Review> Reviews { get; private set; } = new List<Review>();

        protected UserProfile() { }

        public UserProfile(Guid identityUserId, UserRole role, string fullName = "", string email = "", string phoneNumber = "")
        {
            if (identityUserId == Guid.Empty)
                throw new ArgumentException("Identity user id is required.", nameof(identityUserId));

            IdentityUserId = identityUserId;
            Role = role;
            FullName = fullName;
            Email = email;
            PhoneNumber = phoneNumber;
        }

        public void UpdateIdentityData(string fullName, string email, string phoneNumber)
        {
            FullName = fullName;
            Email = email;
            PhoneNumber = phoneNumber;
            SetUpdated();
        }

        public void ChangeRole(UserRole newRole)
        {
            if (Role == newRole)
                return;

            Role = newRole;
            SetUpdated();
        }
    }
}
