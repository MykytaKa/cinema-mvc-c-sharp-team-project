using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class Cinema
    {
        [Key]
        public int Cinema_ID { get; set; }

        public string Cinema_Name { get; set; }
        public string City { get; set; }
        public string Location { get; set; }

        public int Region_ID { get; set; }
        public Region Region { get; set; }

        public ICollection<Hall> Halls { get; set; }
    }
}
