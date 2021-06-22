using System;
using System.Collections.Generic;

namespace Zealous.Models.DB
{
    public partial class Flight
    {
        public Flight()
        {
            Booking = new HashSet<Booking>();
            Notifications = new HashSet<Notifications>();
        }

        public long Id { get; set; }
        public long PilotId { get; set; }
        public string FlightTo { get; set; }
        public long DepartureAirport { get; set; }
        public long DestinationAirport { get; set; }
        public long NumberOfLeftSeats { get; set; }
        public long NumberOfRightSeats { get; set; }
        public long NumberOfRearSeats { get; set; }
        public long TotalLeftSeats { get; set; }
        public long TotalRightSeats { get; set; }
        public long TotalRearSeats { get; set; }
        public long CostOfFlight { get; set; }
        public string UsuableWeightAvailable { get; set; }
        public DateTime DateOfFlight { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime? DateUpdated { get; set; }
        public bool? Active { get; set; }
        public string Plane { get; set; }
        public string FlightType { get; set; }

        public Airports DepartureAirportNavigation { get; set; }
        public Airports DestinationAirportNavigation { get; set; }
        public User Pilot { get; set; }
        public ICollection<Booking> Booking { get; set; }
        public ICollection<Notifications> Notifications { get; set; }
    }
}
