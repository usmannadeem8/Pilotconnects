using System;
using System.Collections.Generic;

namespace Zealous.Models.DB
{
    public partial class Booking
    {
        public long Id { get; set; }
        public long FlightId { get; set; }
        public long PilotId { get; set; }
        public long MemberId { get; set; }
        public long LeftSeat { get; set; }
        public long RightSeat { get; set; }
        public long RearSeat { get; set; }
        public long CostOfSeat { get; set; }
        public long TotalCost { get; set; }
        public int Status { get; set; }
        public DateTime? DateAdded { get; set; }

        public Flight Flight { get; set; }
        public User Member { get; set; }
        public User Pilot { get; set; }
    }
}
