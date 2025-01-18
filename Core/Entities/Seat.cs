using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class Seat
    {
        [Key]
        public int Seat_ID { get; set; }

        public int Column { get; set; }
        public int Row { get; set; }

        public int Hall_ID { get; set; }
        public Hall Hall { get; set; }

        public ICollection<Ticket> Tickets { get; set; }
    }
}
