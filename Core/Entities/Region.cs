using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class Region
    {
        [Key]
        public int Region_ID { get; set; }

        public string Region_Name { get; set; }

        public ICollection<Cinema> Cinemas { get; set; }
    }
}
