using System;
using System.Collections.Generic;

namespace web.Models
{
    public class Users
    {
        public int ID { get; set; }
        public string LastName { get; set; }
        public string FirstMidName { get; set; }
        public DateTime LunchTime { get; set; }

        public ICollection<Friends> Friends { get; set; }
    }
}