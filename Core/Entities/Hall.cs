using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class Hall
    {
        [Key]
        public int Hall_ID { get; set; }

        public string Hall_Name { get; set; }
        public int Number_Of_Seats { get; set; }

        public int Cinema_ID { get; set; }
        public Cinema Cinema { get; set; }

        public ICollection<Seat> Seats { get; set; }
        public ICollection<Session> Sessions { get; set; }
    }
}
