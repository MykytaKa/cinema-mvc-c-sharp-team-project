using System.Collections.Generic;

namespace Web.Models
{
    public class FilmStatisticListViewModel
    {
        public List<FilmStatisticViewModel> Statistics { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string SortBy { get; set; }
    
    }
}
