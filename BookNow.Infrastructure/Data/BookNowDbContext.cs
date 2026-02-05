using BookNow.Domain.Entities;
using BookNow.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookNow.Infrastructure.Data
{
    public class BookNowDbContext
        : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public BookNowDbContext(DbContextOptions<BookNowDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
        public DbSet<Workshop> Workshops => Set<Workshop>();
        public DbSet<Appointment> Appointments => Set<Appointment>();
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Message> Messages { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // USER PROFILE
            modelBuilder.Entity<UserProfile>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.IdentityUserId).IsRequired();
                entity.Property(e => e.Role).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();

                entity.HasIndex(e => e.IdentityUserId).IsUnique();

                // Identity
                entity.HasOne<ApplicationUser>()
                    .WithOne()
                    .HasForeignKey<UserProfile>(e => e.IdentityUserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.Workshops)
                    .WithOne(w => w.MechanicProfile)
                    .HasForeignKey(w => w.MechanicProfileId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.Appointments)
                    .WithOne()
                    .HasForeignKey(a => a.ClientProfileId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // WORKSHOP
            modelBuilder.Entity<Workshop>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.Address).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Latitude).IsRequired();
                entity.Property(e => e.Longitude).IsRequired();
                entity.Property(e => e.IsVerified).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();

                entity.HasMany(e => e.Appointments)
                    .WithOne()
                    .HasForeignKey(a => a.WorkshopId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.MechanicProfileId);
                entity.HasIndex(e => new { e.Latitude, e.Longitude });
            });

            // APPOINTMENT
            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.AppointmentAt).IsRequired();
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.IssueDescription).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.CreatedAt).IsRequired();

                entity.HasIndex(e => e.ClientProfileId);
                entity.HasIndex(e => e.WorkshopId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.AppointmentAt);
            });
            //Review
            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Rating)
                    .IsRequired();

                entity.Property(e => e.Comment)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.CreatedAt)
                    .IsRequired();

                entity.HasOne(e => e.ClientProfile)
                    .WithMany(u => u.Reviews)
                    .HasForeignKey(e => e.ClientProfileId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Workshop)
                    .WithMany(w => w.Reviews)
                    .HasForeignKey(e => e.WorkshopId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Appointment)
                    .WithOne(a => a.Review)
                    .HasForeignKey<Review>(e => e.AppointmentId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.WorkshopId);
                entity.HasIndex(e => e.ClientProfileId);
                entity.HasIndex(e => e.AppointmentId)
                    .IsUnique();
            });

            modelBuilder.Entity<Conversation>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(e => e.Appointment)
                    .WithOne(a => a.Conversation)
                    .HasForeignKey<Conversation>(e => e.AppointmentId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.AppointmentId).IsUnique();
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.Property(e => e.SenderType)
                    .IsRequired();

                entity.Property(e => e.IsRead)
                    .IsRequired();

                entity.HasOne(e => e.Conversation)
                    .WithMany(c => c.Messages)
                    .HasForeignKey(e => e.ConversationId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.ConversationId);
                entity.HasIndex(e => e.SenderProfileId);
            });


        }
    }
}
