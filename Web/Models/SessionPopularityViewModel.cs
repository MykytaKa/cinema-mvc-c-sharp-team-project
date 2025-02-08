
namespace Web.Models
{
    public class SessionPopularityViewModel
    {
        public string TimePeriod { get; set; }
        public int TotalBookings { get; set; }
    }

    public class SessionPopularityListViewModel
    {
        public List<SessionPopularityViewModel> Statistics { get; set; }
        public DateTime? DateTimeBeg { get; set; }
        public DateTime? DateTimeEnd { get; set; }
    }
}