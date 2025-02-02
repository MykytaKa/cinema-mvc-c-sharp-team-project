using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces.Services;

namespace Infrastructure.Services
{
    public class TicketService : ITicketService
    {
        public IEnumerable<Ticket> GetReservedTickets(int userId)
        {
            
        }

        public IEnumerable<Ticket> GetUsedTickets(int userId)
        {
            throw new NotImplementedException();
        }
    }
}
