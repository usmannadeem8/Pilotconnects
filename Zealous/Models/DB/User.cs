using System;
using System.Collections.Generic;

namespace Zealous.Models.DB
{
    public partial class User
    {
        public User()
        {
            BookingMember = new HashSet<Booking>();
            BookingPilot = new HashSet<Booking>();
            ChatPermissionUser1Navigation = new HashSet<ChatPermission>();
            ChatPermissionUser2Navigation = new HashSet<ChatPermission>();
            ChatReceiver = new HashSet<Chat>();
            ChatSender = new HashSet<Chat>();
            Flight = new HashSet<Flight>();
            NotificationsMember = new HashSet<Notifications>();
            NotificationsPilot = new HashSet<Notifications>();
        }

        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int EmailConfirmed { get; set; }
        public string PilotLicenseNumber { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string PhoneNumber { get; set; }
        public long AirportId { get; set; }
        public DateTime DateCreated { get; set; }
        public int Faa { get; set; }
        public string CertificateType { get; set; }
        public string PilotRating { get; set; }
        public string AboutMe { get; set; }
        public string CareerGoals { get; set; }
        public bool? Active { get; set; }
        public string ImagePath { get; set; }
        public string Address { get; set; }

        public Airports Airport { get; set; }
        public ICollection<Booking> BookingMember { get; set; }
        public ICollection<Booking> BookingPilot { get; set; }
        public ICollection<ChatPermission> ChatPermissionUser1Navigation { get; set; }
        public ICollection<ChatPermission> ChatPermissionUser2Navigation { get; set; }
        public ICollection<Chat> ChatReceiver { get; set; }
        public ICollection<Chat> ChatSender { get; set; }
        public ICollection<Flight> Flight { get; set; }
        public ICollection<Notifications> NotificationsMember { get; set; }
        public ICollection<Notifications> NotificationsPilot { get; set; }
    }
}
