using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class Film
    {
        [Key]
        public int Film_ID { get; set; }

        public string Film_Name { get; set; }
        public string Poster { get; set; }
        public string Trailer { get; set; }
        public string Description { get; set; }
        public string Actors { get; set; }
        public decimal Rating { get; set; }
        public DateTime Release_Date { get; set; }
        public TimeSpan Duration_Of_The_Movie { get; set; }
        public string Age_Rating { get; set; }
        public string Director { get; set; }

        public int Genre_ID { get; set; }
        public Genre Genre { get; set; }

        public ICollection<Session> Sessions { get; set; }
    }
}
