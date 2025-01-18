using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class Ticket
    {
        [Key]
        public int Ticket_ID { get; set; }

        public int Booking_ID { get; set; }
        public Booking Booking { get; set; }

        public int Seat_ID { get; set; }
        public Seat Seat { get; set; }
    }
}
