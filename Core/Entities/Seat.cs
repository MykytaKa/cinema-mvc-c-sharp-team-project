using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public class Seat
    {
        [Key]
        public int Id { get; set; }

        public int Column { get; set; }
        public int Row { get; set; }

        public int HallId { get; set; }
        [ForeignKey("HallId")]
        public Hall? Hall { get; set; }

        public ICollection<Ticket> Tickets { get; set; }
    }
}
