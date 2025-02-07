
using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Models;

namespace Web.Controllers
{
    [Authorize]
    public class BookingController(IBookingService bookingService, IMapper mapper) : Controller
    {
        [HttpGet("BookSession")]
        public async Task<IActionResult> BookSession(int sessionId)
        {
            BookSessionDto dto = await bookingService.PrepareBookingAsync(sessionId);
            BookSessionViewModel viewModel = mapper.Map<BookSessionViewModel>(dto);

            return View("Booking", viewModel);
        }
        
        [HttpPost("ConfirmBooking")]
        public async Task<IActionResult> ConfirmBooking(BookSessionViewModel model)
        {
            var bookingDto = mapper.Map<BookSessionDto>(model);
            var result = await bookingService.ConfirmBookingAsync(bookingDto, User);
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