using Core.Entities;
using Core.Interfaces;
using Infrastructure.Repositories;
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

        public async Task<Film> FilmInfoAsync(int filmId)
        {
            var films = await _unitofwork.Repository<Film>().GetAsync(
                f => f.Id == filmId,  
                includeProperties: "Genres,Actors"  
            );

            return films.FirstOrDefault(); 
        }
    }
}