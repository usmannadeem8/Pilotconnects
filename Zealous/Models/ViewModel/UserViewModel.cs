using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Zealous.Models.ViewModel
{
    public class UserViewModel
    {
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
        public string AirportName { get; set; }

        
    }
}
