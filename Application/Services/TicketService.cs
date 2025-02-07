using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities;
using Application.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Application.Services
{
    public class TicketService : ITicketService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TicketService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Ticket>> GetTickets(int userId, string status)
        {
            var userBookings = await _unitOfWork.Repository<Booking>().GetAsync(
                filter: b => b.UserId == userId && b.Status.Name == status
            );

            var bookingIds = userBookings.Select(b => b.Id).ToList();

            if (!bookingIds.Any())
            {
                return new List<Ticket>();
            }

            var reservedTickets = await _unitOfWork.Repository<Ticket>().GetAsync(
                filter: t => bookingIds.Contains(t.BookingId),
                includeProperties: "Seat,Seat.Hall,Booking,Booking.Session,Booking.Session.Film"
            );

            return reservedTickets;
        }
    }
}
