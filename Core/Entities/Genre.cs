using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class Genre
    {
        [Key]
        public int Genre_ID { get; set; }

        public string Genre_Name { get; set; }

        public ICollection<Film> Films { get; set; }
    }
}
