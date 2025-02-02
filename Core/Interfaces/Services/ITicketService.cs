using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities;

namespace Core.Interfaces.Services
{
    public interface ITicketService
    {
        IEnumerable<Ticket> GetReservedTickets(int userId);
        IEnumerable<Ticket> GetUsedTickets(int userId);
    }
}
