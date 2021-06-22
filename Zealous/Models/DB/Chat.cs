using System;
using System.Collections.Generic;

namespace Zealous.Models.DB
{
    public partial class Chat
    {
        public long Id { get; set; }
        public long SenderId { get; set; }
        public long ReceiverId { get; set; }
        public string Message { get; set; }
        public DateTime DateAdded { get; set; }
        public int IsSeen { get; set; }

        public User Receiver { get; set; }
        public User Sender { get; set; }
    }
}
