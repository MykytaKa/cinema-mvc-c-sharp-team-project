using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class Session
    {
        [Key]
        public int Session_ID { get; set; }

        public DateTime Date_Time_Beg { get; set; }
        public DateTime Date_Time_End { get; set; }
        public decimal Price { get; set; }

        public int Film_ID { get; set; }
        public Film Film { get; set; }

        public int Hall_ID { get; set; }
        public Hall Hall { get; set; }

        public ICollection<Booking> Bookings { get; set; }
    }
}
