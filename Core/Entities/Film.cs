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

        public string Name { get; set; }
        public string PosterURL { get; set; }
        public string TrailerURL { get; set; }
        public string Description { get; set; }
        public ICollection<Actor> Actors { get; set; }
        public decimal Rating { get; set; }
        public DateTime ReleaseRate { get; set; }
        public TimeSpan Duration { get; set; }
        public string AgeRating { get; set; }
        public string Director { get; set; }
        public ICollection<Genre> Genres { get; set; }
        public ICollection<Session> Sessions { get; set; }
    }
}
