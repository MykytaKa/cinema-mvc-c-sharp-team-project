using Core.Entities;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IFilmInfoService
    {
        Task<Film> GetFilmByIDAsync(object id);
    }
}
