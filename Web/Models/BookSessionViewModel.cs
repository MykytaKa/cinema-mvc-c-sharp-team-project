namespace Web.Models;

public class BookSessionViewModel
{
    public int SessionId { get; set; }
    public string FilmName { get; set; }
    public string HallName { get; set; }
    public DateTime SessionDate { get; set; }
    public decimal SessionPrice { get; set; }
    public List<SeatViewModel> AvailableSeats { get; set; } = new();
    public List<int> SelectedSeats { get; set; } = new();
}