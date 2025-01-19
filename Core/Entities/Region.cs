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
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<Cinema> Cinemas { get; set; }
    }
}
