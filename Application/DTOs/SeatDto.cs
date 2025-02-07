namespace Application.DTOs;

public class SeatDto
{
    public int SeatId { get; set; }
    public int Row { get; set; }
    public int Column { get; set; }

    public bool IsBooked { get; set; }
}