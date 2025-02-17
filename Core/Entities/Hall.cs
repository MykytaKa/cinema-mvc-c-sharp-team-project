using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public class Hall
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
        public int NumberOfSeats { get; set; }
        public ICollection<Seat> Seats { get; set; }
        public ICollection<Session> Sessions { get; set; }
    }
}
