using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class NewFilmDTO
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string PosterURL { get; set; }
        public required DateTime RealeaseDate { get; set; }

    }
}
