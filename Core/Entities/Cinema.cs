using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public class Cinema
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string Building { get; set; }
        public int RegionId { get; set; }
        [ForeignKey("RegionId")]
        public Region Region { get; set; }

        public ICollection<Hall> Halls { get; set; }
    }
}
