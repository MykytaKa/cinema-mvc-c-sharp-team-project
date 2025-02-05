using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class TicketViewModel
    {
        [Required]
        public DateTime DateTimeBeg { get; set; }

        [Required]
        public string FilmTitle { get; set; }

        [Required]
        public string FilmPosterURL { get; set; }

        [Required]
        public int Column {  get; set; }

        [Required]
        public int Row { get; set; }

        [Required]
        public string HallName { get; set; }

        [Required]
        public decimal Price { get; set; }
    }
}
