using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }

        public DateTime DateTime { get; set; }
        public decimal Price { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

        public int SessionId { get; set; }
        [ForeignKey("SessionId")]
        public Session Session { get; set; }

        public int StatusId { get; set; }
        [ForeignKey("StatusId")]
        public Status Status { get; set; }

        public ICollection<Ticket> Tickets { get; set; }
    }
}
