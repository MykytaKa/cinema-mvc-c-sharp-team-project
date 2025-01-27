using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class FilmRecoViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string PosterUrl { get; set; } = string.Empty;
    }
}
