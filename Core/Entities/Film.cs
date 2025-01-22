using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public class Film
    {
        [Key]
        public int Id { get; set; }

        public string Film_Name { get; set; }
        public string PosterURL { get; set; }
        public string TrailerURL { get; set; }
        public string Description { get; set; }
        public ICollection<Actor> Actors { get; set; }
        public decimal Rating { get; set; }
        public DateTime Release_Date { get; set; }
        public TimeSpan Duration_Of_The_Movie { get; set; }
        public string Age_Rating { get; set; }
        public string Director { get; set; }

        public int GenreId { get; set; }
        [ForeignKey("GenreId")]
        public Genre Genre { get; set; }

        public ICollection<Session> Sessions { get; set; }
    }
}
