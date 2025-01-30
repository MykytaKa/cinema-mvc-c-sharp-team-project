namespace Core.Models;

public class BookingViewModel
{
    public int Id { get; set; }
    public DateTime DateTime { get; set; }
    public decimal Price { get; set; }
    public string FilmName { get; set; }
    public string HallName { get; set; }
    public DateTime SessionStart { get; set; }
    public DateTime SessionEnd { get; set; }
    public string Status { get; set; }
    public List<string> Seats { get; set; }
}