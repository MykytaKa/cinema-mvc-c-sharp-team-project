namespace Core.Models;

public class CreateBookingDto
{
    public int UserId { get; set; }
    public int SessionId { get; set; }
    public List<int> SeatIds { get; set; }
}