using Core.Entities;
using Application.Interfaces;

namespace Application.Services
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