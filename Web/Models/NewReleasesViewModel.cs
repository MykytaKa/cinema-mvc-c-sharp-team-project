using Application.DTOs;

namespace Web.Models
{
    public class NewReleasesViewModel
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public int? PrevMonth { get; set; }
        public int? PrevYear { get; set; }
        public int NextMonth { get; set; }
        public int NextYear { get; set; }
        public List<NewFilmDTO> FilmDTOs { get; set; }
    }
}
