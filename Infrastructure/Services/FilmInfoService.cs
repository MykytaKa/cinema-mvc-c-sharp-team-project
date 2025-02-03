using Core.Entities;
using Core.Interfaces;
using Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class FilmInfoService : IFilmInfoService
    {
        private readonly IUnitOfWork _unitofwork;

        public FilmInfoService(IUnitOfWork unitofwork)
        {
            _unitofwork = unitofwork;
        }

        public async Task<Film> GetFilmByIDAsync(object id)
        {
            return await _unitofwork.Repository<Film>().GetByIDAsync(id);
        }
    }
}