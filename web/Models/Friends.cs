using System;

namespace web.Models
{
    
    public class Friends
    {
        public int FriendsID { get; set; }

        public string FriendName {get; set;}
        public int UserID { get; set; }
        
        public DateTime LunchTime { get; set; }

        public Users Users { get; set; }
    }
}