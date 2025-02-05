using Core.Models;

namespace Web.Models
{
    public class NewReleasesViewModel
    {
        public int Month {  get; set; }
        public int Year { get; set; }
        public List<NewFilmDTO> FilmDTOs { get; set; }
    }
}
