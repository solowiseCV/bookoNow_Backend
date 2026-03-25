using BookNow.Domain.Entities;
using BookNow.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookNow.Infrastructure.Data
{
    public class BookNowDbContext(DbContextOptions<BookNowDbContext> options)
        : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
    {
        public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
        public DbSet<Workshop> Workshops => Set<Workshop>();
        public DbSet<WorkshopImage> WorkshopImages => Set<WorkshopImage>();
        public DbSet<Appointment> Appointments => Set<Appointment>();
        public DbSet<Review> Reviews => Set<Review>();
        public DbSet<Conversation> Conversations => Set<Conversation>();
        public DbSet<Message> Messages => Set<Message>();
        public DbSet<Shop> Shops => Set<Shop>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<AppointmentAttachment> AppointmentAttachments => Set<AppointmentAttachment>();
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<RevokedToken> RevokedTokens { get; set; }
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

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasMaxLength(500);

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
                entity.HasIndex(e => e.Type);
            });

            // WORKSHOP IMAGE
            modelBuilder.Entity<WorkshopImage>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .ValueGeneratedNever();

                entity.Property(e => e.Url)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.PublicId)
                    .HasMaxLength(200);

                entity.HasOne(e => e.Workshop)
                    .WithMany(w => w.GalleryImages)
                    .HasForeignKey(e => e.WorkshopId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.WorkshopId);
            });

            // APPOINTMENT
            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.AppointmentAt).IsRequired();
                entity.Property(e => e.Status).IsRequired();

                entity.Property(e => e.IssueDescription)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.CreatedAt).IsRequired();

                entity.HasIndex(e => e.ClientProfileId);
                entity.HasIndex(e => e.WorkshopId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.AppointmentAt);
            });

            // REVIEW
            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Rating).IsRequired();

                entity.Property(e => e.Comment)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.CreatedAt).IsRequired();

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
                entity.HasIndex(e => e.AppointmentId).IsUnique();
            });

            // CONVERSATION
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

            // MESSAGE
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

            // SHOP
            modelBuilder.Entity<Shop>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.IsSubscribed).IsRequired();

                entity.HasOne(e => e.Owner)
                    .WithOne()
                    .HasForeignKey<Shop>(e => e.OwnerId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // PRODUCT
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Price)
                    .HasPrecision(18, 2);

                entity.HasOne(e => e.Shop)
                    .WithMany(s => s.Products)
                    .HasForeignKey(p => p.ShopId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ORDER
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.TotalAmount)
                    .HasPrecision(18, 2);

                entity.HasMany(e => e.Items)
                    .WithOne(oi => oi.Order)
                    .HasForeignKey(oi => oi.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });


            // ORDER ITEM
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.UnitPrice)
                    .HasPrecision(18, 2);

                entity.Property(e => e.Quantity)
                    .IsRequired();

                entity.HasOne(e => e.Order)
                    .WithMany(o => o.Items)
                    .HasForeignKey(e => e.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Product)
                    .WithMany()
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.OrderId);
                entity.HasIndex(e => e.ProductId);
            });

            // PAYMENT

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Reference)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Amount)
                    .HasPrecision(18, 2);

                entity.Property(e => e.SystemCommission)
                    .HasPrecision(18, 2);

                entity.HasOne(e => e.Order)
                    .WithOne(o => o.Payment)
                    .HasForeignKey<Payment>(e => e.OrderId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.Shop)
                    .WithMany()
                    .HasForeignKey(e => e.ShopId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.Workshop)
                    .WithMany()
                    .HasForeignKey(e => e.WorkshopId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // NOTIFICATION
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.UserId).IsRequired();
                
                entity.Property(e => e.Message)
                    .IsRequired()
                    .HasMaxLength(1500);

                entity.Property(e => e.IsRead).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();

                entity.HasIndex(e => e.UserId);
            });
        }
    }
}

