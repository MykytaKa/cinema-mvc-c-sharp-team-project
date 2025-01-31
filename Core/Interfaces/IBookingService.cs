using Core.Entities;
using Core.Models;


namespace Core.Interfaces;

public interface IBookingService
{
    Task<List<BookingViewModel>> GetUserBookingsAsync(int userId);
    Task<BookSessionViewModel> PrepareBookingAsync(int sessionId);
    Task<Booking> ConfirmBookingAsync(CreateBookingDto bookingDto);
}