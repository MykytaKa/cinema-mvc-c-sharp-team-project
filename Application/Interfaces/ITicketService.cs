using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities;

namespace Application.Interfaces
{
    public interface ITicketService
    {
        Task<IEnumerable<Ticket>> GetTickets(int userId, string status);
    }
}
