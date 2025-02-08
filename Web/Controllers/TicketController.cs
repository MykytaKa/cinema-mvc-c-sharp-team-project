using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Web.Models;

namespace Web.Controllers
{
    public class TicketController : Controller
    {
        private readonly ITicketService _ticketService;

        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        public async Task<IActionResult> ShowUserTickets(string status = "Reserved")
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized("Invalid User ID.");
            }

            var tickets = await _ticketService.GetTickets(userId, status);

            var ticketViewModels = tickets
                .OrderBy(t => t.Booking.DateTime)
                .Select(ticket => new TicketViewModel
                {
                    DateTimeBeg = ticket.Booking.Session.DateTimeBeg,
                    FilmTitle = ticket.Booking.Session.Film.Name,
                    FilmPosterURL = ticket.Booking.Session.Film.PosterURL,
                    Column = ticket.Seat.Column,
                    Row = ticket.Seat.Row,
                    HallName = ticket.Seat.Hall.Name,
                    Price = ticket.Booking.Session.Price
                }).ToList();

            return View("Ticket", ticketViewModels);
        }
    }
}
