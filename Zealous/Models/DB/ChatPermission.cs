using System;
using System.Collections.Generic;

namespace Zealous.Models.DB
{
    public partial class ChatPermission
    {
        public long Id { get; set; }
        public long User1 { get; set; }
        public long User2 { get; set; }
        public string LastMessage { get; set; }
        public DateTime? LastMessageTime { get; set; }
        public long IsSeen { get; set; }
        public DateTime? DateAdded { get; set; }

        public User User1Navigation { get; set; }
        public User User2Navigation { get; set; }
    }
}
