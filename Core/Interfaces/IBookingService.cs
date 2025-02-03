using System.Security.Claims;
using Core.Entities;
using Core.Models;


namespace Core.Interfaces;

public interface IBookingService
{
    Task<BookSessionViewModel> PrepareBookingAsync(int sessionId);
    Task<Booking> ConfirmBookingAsync(CreateBookingDto bookingDto);
    Task<(bool IsSuccess, string ErrorMessage, Booking? Booking)> ConfirmBookingAsync(BookSessionViewModel model, ClaimsPrincipal user);
}