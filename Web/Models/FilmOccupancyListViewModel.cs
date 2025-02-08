using System.Collections.Generic;

namespace Web.Models
{
    public class FilmOccupancyListViewModel
    {
        public List<FilmOccupancyViewModel> Statistics { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string SortBy { get; set; }
        public DateTime? DateTimeBeg { get; set; }
        public DateTime? DateTimeEnd { get; set; }
    }
}
