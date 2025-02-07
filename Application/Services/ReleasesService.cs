using Core.Entities;
using Application.Interfaces;
using Core.Models;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class ReleasesService : IReleasesService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ReleasesService> _logger;
        public ReleasesService(IUnitOfWork unitOfWork, ILogger<ReleasesService> logger) 
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IEnumerable<NewFilmDTO>> GetNewReleases(int month, int year)
        {
            var newFilms = await _unitOfWork.Repository<Film>().GetAsync(
                filter: f => f.ReleaseRate.Month == month && f.ReleaseRate.Year == year
                );
            
            return newFilms
                .OrderBy( f => f.ReleaseRate.Month )
                .Select(nf => new NewFilmDTO
                { 
                    Id = nf.Id,
                    Name = nf.Name,
                    PosterURL = nf.PosterURL,
                    RealeaseDate = nf.ReleaseRate
                });
        }
    }
}
