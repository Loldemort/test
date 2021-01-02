using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace web.Models
{
    public class Users
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]

        [Key]
        public long userId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        
        [DisplayFormat(DataFormatString="{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan lunchTime { get; set; }
        public string Location { get; set; }
        public string PictureUrl { get; set; }
        
    }
}