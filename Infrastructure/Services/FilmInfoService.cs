using Core.Entities;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class FilmInfoService : IFilmInfoService
    {
        private readonly IGenericRepository<Film> _filminfo;

        public FilmInfoService(IGenericRepository<Film> filminfo)
        {
            _filminfo = filminfo;
        }

        public async Task<Film> GetFilmByIDAsync(object id)
        {
            return await _filminfo.GetByIDAsync(id);
        }
    }
}
