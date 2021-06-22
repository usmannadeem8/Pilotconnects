using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Zealous.Models.DB
{
    public partial class ZealousContext : DbContext
    {
        public ZealousContext()
        {
        }

        public ZealousContext(DbContextOptions<ZealousContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Airports> Airports { get; set; }
        public virtual DbSet<Booking> Booking { get; set; }
        public virtual DbSet<Chat> Chat { get; set; }
        public virtual DbSet<ChatPermission> ChatPermission { get; set; }
        public virtual DbSet<Configure> Configure { get; set; }
        public virtual DbSet<Flight> Flight { get; set; }
        public virtual DbSet<Notifications> Notifications { get; set; }
        public virtual DbSet<Payment> Payment { get; set; }
        public virtual DbSet<User> User { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
               // optionsBuilder.UseSqlServer("Server=DESKTOP-TT459TV;Database=Zealous;Trusted_Connection=True;");
               optionsBuilder.UseSqlServer("Server=S107-180-92-194;Database=Zealous;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Airports>(entity =>
            {
                entity.Property(e => e.Airport).IsRequired();
            });

            modelBuilder.Entity<Booking>(entity =>
            {
                entity.Property(e => e.DateAdded)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Flight)
                    .WithMany(p => p.Booking)
                    .HasForeignKey(d => d.FlightId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Booking_Flight");

                entity.HasOne(d => d.Member)
                    .WithMany(p => p.BookingMember)
                    .HasForeignKey(d => d.MemberId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Booking_Member");

                entity.HasOne(d => d.Pilot)
                    .WithMany(p => p.BookingPilot)
                    .HasForeignKey(d => d.PilotId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Booking_Pilot");
            });

            modelBuilder.Entity<Chat>(entity =>
            {
                entity.Property(e => e.DateAdded)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Message)
                    .IsRequired()
                    .IsUnicode(false);

                entity.HasOne(d => d.Receiver)
                    .WithMany(p => p.ChatReceiver)
                    .HasForeignKey(d => d.ReceiverId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Chat_User1");

                entity.HasOne(d => d.Sender)
                    .WithMany(p => p.ChatSender)
                    .HasForeignKey(d => d.SenderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Chat_User");
            });

            modelBuilder.Entity<ChatPermission>(entity =>
            {
                entity.Property(e => e.DateAdded)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.LastMessage).IsUnicode(false);

                entity.Property(e => e.LastMessageTime).HasColumnType("datetime");

                entity.HasOne(d => d.User1Navigation)
                    .WithMany(p => p.ChatPermissionUser1Navigation)
                    .HasForeignKey(d => d.User1)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ChatPermission_User");

                entity.HasOne(d => d.User2Navigation)
                    .WithMany(p => p.ChatPermissionUser2Navigation)
                    .HasForeignKey(d => d.User2)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ChatPermission_User1");
            });

            modelBuilder.Entity<Configure>(entity =>
            {
                entity.Property(e => e.Mc).HasColumnName("MC");

                entity.Property(e => e.Md)
                    .HasColumnName("MD")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<Flight>(entity =>
            {
                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.DateAdded).HasColumnType("datetime");

                entity.Property(e => e.DateOfFlight).HasColumnType("datetime");

                entity.Property(e => e.DateUpdated).HasColumnType("datetime");

                entity.Property(e => e.FlightType).HasMaxLength(50);

                entity.HasOne(d => d.DepartureAirportNavigation)
                    .WithMany(p => p.FlightDepartureAirportNavigation)
                    .HasForeignKey(d => d.DepartureAirport)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Flight_Airport2");

                entity.HasOne(d => d.DestinationAirportNavigation)
                    .WithMany(p => p.FlightDestinationAirportNavigation)
                    .HasForeignKey(d => d.DestinationAirport)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Flight_Airports");

                entity.HasOne(d => d.Pilot)
                    .WithMany(p => p.Flight)
                    .HasForeignKey(d => d.PilotId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Flight_User");
            });

            modelBuilder.Entity<Notifications>(entity =>
            {
                entity.Property(e => e.DateAdded)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Flight)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.FlightId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Notifications_Flight");

                entity.HasOne(d => d.Member)
                    .WithMany(p => p.NotificationsMember)
                    .HasForeignKey(d => d.MemberId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Notifications_Member");

                entity.HasOne(d => d.Pilot)
                    .WithMany(p => p.NotificationsPilot)
                    .HasForeignKey(d => d.PilotId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Notifications_Pilot");
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.Property(e => e.FromDate).HasColumnType("datetime");

                entity.Property(e => e.Status).HasDefaultValueSql("((1))");

                entity.Property(e => e.ToDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.CertificateType).HasMaxLength(50);

                entity.Property(e => e.ConfirmPassword)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.DateCreated)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.PhoneNumber)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.PilotLicenseNumber).IsRequired();

                entity.HasOne(d => d.Airport)
                    .WithMany(p => p.User)
                    .HasForeignKey(d => d.AirportId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_User_User");
            });
        }
    }
}
