using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class Booking
    {
        [Key]
        public int Booking_ID { get; set; }

        public DateTime Date_Time { get; set; }
        public decimal Sum { get; set; }

        public int User_ID { get; set; }
        public User User { get; set; }

        public int Session_ID { get; set; }
        public Session Session { get; set; }

        public int Status_ID { get; set; }
        public Status Status { get; set; }

        public ICollection<Ticket> Tickets { get; set; }
    }
}
