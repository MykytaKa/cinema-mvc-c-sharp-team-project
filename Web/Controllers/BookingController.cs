using System.Security.Claims;
using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Models;

namespace Web.Controllers;

[Authorize]
public class BookingController : Controller
{
    private readonly IBookingService _bookingService;

    public BookingController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpGet("Booking/BookSession")]
    public async Task<IActionResult> BookSession(int sessionId)
    {
        var model = await _bookingService.PrepareBookingAsync(sessionId);
        if (model == null)
            return NotFound("Сеанс не знайдено");

        return View("Booking", model); 
    }
    
    [HttpPost("Booking/ConfirmBooking")]
    public async Task<IActionResult> ConfirmBooking(BookSessionViewModel model)
    {
        if (model.SelectedSeats == null || !model.SelectedSeats.Any())
        {
            ModelState.AddModelError("", " Ви не вибрали місця!");
            return View("Booking", model);
        }

        // отримання UserId
        string? userIdClaim = User.FindFirstValue("UserId");
    
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            Console.WriteLine($" Помилка: UserId не є числом! Отримано: {userIdClaim}");
            return Unauthorized(new { message = "Помилка авторизації!" });
        }

        Console.WriteLine($" Авторизований користувач: {userId}");

        var bookingDto = new CreateBookingDto
        {
            UserId = userId,
            SessionId = model.SessionId,
            SeatIds = model.SelectedSeats
        };

        var booking = await _bookingService.ConfirmBookingAsync(bookingDto);
        if (booking == null)
        {
            ModelState.AddModelError("", " Не вдалося забронювати. Спробуйте ще раз.");
            return View("Booking", model);
        }

        Console.WriteLine(" Бронювання успішне! Перенаправлення на сторінку бронювань...");
        return RedirectToAction("ShowUserTickets", "Ticket");
    }
}