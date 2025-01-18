using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class Status
    {
        [Key]
        public int Status_ID { get; set; }

        public string Status_Name { get; set; }

        public ICollection<Booking> Bookings { get; set; }
    }
}
