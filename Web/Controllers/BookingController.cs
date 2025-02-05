using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet("BookSession")]
        public async Task<IActionResult> BookSession(int sessionId)
        {
            var model = await _bookingService.PrepareBookingAsync(sessionId);

            return View("Booking", model);
        }
        
        [HttpPost("ConfirmBooking")]
        public async Task<IActionResult> ConfirmBooking(BookSessionViewModel model)
        {
            var result = await _bookingService.ConfirmBookingAsync(model, User);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.ErrorMessage);
                return View("Booking", model);
            }

            Console.WriteLine("Бронювання успішне! Перенаправлення на сторінку бронювань...");
            return RedirectToAction("ShowUserTickets", "Ticket");
        }
    }
}