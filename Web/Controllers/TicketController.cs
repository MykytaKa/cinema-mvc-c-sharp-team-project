using Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class TicketController : Controller
    {
        private readonly ITicketService _ticketService;

        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        public IActionResult ShowUserTickets()
        {
            return View("Ticket");
        }
    }
}
