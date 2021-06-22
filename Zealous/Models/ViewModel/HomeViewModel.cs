using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zealous.Models.DB;

namespace Zealous.Models.ViewModel
{
    public class HomeViewModel
    {
        public IEnumerable<Flight> Flights { get; set; }
        public IEnumerable<Notifications> Notifications { get; set; }
        public IEnumerable<Booking> Bookings { get; set; }
        public IEnumerable<ChatPermission> ChatPermissions { get; set; }
        public IEnumerable<Chat> Chats { get; set; }

        public IEnumerable<UserViewModel> UserV { get; set; }

        public long FlightCount { get; set; }
        public long NotificationCount { get; set; }
        public long BookingCount { get; set; }
        public long NewMessageCount { get; set; }

        public Chat Chat { get; set; }

        public Flight Flight { get; set; }

        public User User { get; set; }

        public long CostOfFlight { get; set; }

        public long? CostOfSeat { get; set; }

        public bool IsFirstSeat { get; set; }
    }
}
