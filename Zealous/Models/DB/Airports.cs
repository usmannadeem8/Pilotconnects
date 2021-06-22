using System;
using System.Collections.Generic;

namespace Zealous.Models.DB
{
    public partial class Airports
    {
        public Airports()
        {
            FlightDepartureAirportNavigation = new HashSet<Flight>();
            FlightDestinationAirportNavigation = new HashSet<Flight>();
            User = new HashSet<User>();
        }

        public long Id { get; set; }
        public string Airport { get; set; }
        public string Lat { get; set; }
        public string Lang { get; set; }
        public string State { get; set; }

        public ICollection<Flight> FlightDepartureAirportNavigation { get; set; }
        public ICollection<Flight> FlightDestinationAirportNavigation { get; set; }
        public ICollection<User> User { get; set; }
    }
}
