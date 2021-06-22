using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zealous.Models.DB;

namespace Zealous.Models.ViewModel
{
    public class AdminViewModel
    {
        public long TotalUserCount { get; set; }
        public long ActiveUserCount { get; set; }
        public long InActiveUserCount { get; set; }
        public long FAAUserCount { get; set; }

        public long TotalRevenue { get; set; }
        public long ThisYearRevenue { get; set; }
        public long ThisMonthRevenue { get; set; }
        public long TodayRevenue { get; set; }

        public long TotalAirportCount { get; set; }
        public long TotalBookingCount { get; set; }
        public long TotalFlightsCount { get; set; }
        public long ActiveFlightsCount { get; set; }

        public int MC { get; set; }
        public int MD { get; set; }
        public IEnumerable<User> PaidUsers { get; set; }
        public IEnumerable<User> UnPaidUsers { get; set; }
    }
}
