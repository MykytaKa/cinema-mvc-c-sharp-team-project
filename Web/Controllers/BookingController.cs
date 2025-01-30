using System.Security.Claims;
using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Models;

namespace Web.Controllers;

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

    [Authorize]
    [HttpPost("Booking/ConfirmBooking")]
    public async Task<IActionResult> ConfirmBooking(BookSessionViewModel model)
    {
        if (model.SelectedSeats == null || !model.SelectedSeats.Any())
        {
            ModelState.AddModelError("", " Ви не вибрали місця!");
            return View("Booking", model);
        }
        
        // Перевіряємо, чи користувач авторизований
        if (!User.Identity.IsAuthenticated)
        {
            ModelState.AddModelError("", "Ви повинні увійти в систему для бронювання!");
            return View("Booking", model);
        }

        int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        Console.WriteLine($"➡ Користувач {userId} підтверджує бронювання: SessionId={model.SessionId}, Місця={string.Join(", ", model.SelectedSeats)}");

        var bookingDto = new CreateBookingDto
        {
            UserId = userId, // 🔹 Замінити на реального користувача
            SessionId = model.SessionId,
            SeatIds = model.SelectedSeats
        };

        var booking = await _bookingService.ConfirmBookingAsync(bookingDto);
        if (booking == null)
        {
            ModelState.AddModelError("", " Не вдалося забронювати. Спробуйте ще раз.");
            return View("Booking", model);
        }

        Console.WriteLine("🎉 Бронювання успішне! Перенаправлення на сторінку бронювань...");
        return RedirectToAction("Bookings", new { userId = 1 });
    }
}