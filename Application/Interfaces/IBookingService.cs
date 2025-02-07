using System.Security.Claims;
using Application.DTOs;
using Core.Entities;


namespace Application.Interfaces;

public interface IBookingService
{
    Task<BookSessionDto> PrepareBookingAsync(int sessionId);
    Task<Booking> ConfirmBookingAsync(CreateBookingDto bookingDto);
    Task<(bool IsSuccess, string ErrorMessage, Booking? Booking)> ConfirmBookingAsync(BookSessionDto model, ClaimsPrincipal user);
}