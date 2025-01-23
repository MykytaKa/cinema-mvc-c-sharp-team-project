using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public class Session
    {
        [Key]
        public int Id { get; set; }

        public DateTime DateTimeBeg { get; set; }
        public DateTime DateTimeEnd { get; set; }
        public decimal Price { get; set; }

        public int FilmId { get; set; }
        [ForeignKey("FilmId")]
        public Film Film { get; set; }

        public int HallId { get; set; }
        [ForeignKey("FilmId")]
        public Hall Hall { get; set; }

        public ICollection<Booking> Bookings { get; set; }
    }
}
