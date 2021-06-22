using System;
using System.Collections.Generic;

namespace Zealous.Models.DB
{
    public partial class Payment
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public long Amount { get; set; }
        public int Status { get; set; }
    }
}
