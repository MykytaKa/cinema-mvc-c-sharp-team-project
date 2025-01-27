using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Core.Interfaces.Services;
using Infrastructure.Data;
using MathNet.Numerics.LinearAlgebra;


namespace Infrastructure.Services
{
    public class RecommendationService : IRecommendationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RecommendationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<Film> GetRecommendations(int userId)
        {
            // 1. Get all films the user has visited
            var visitedFilms = GetVisitedFilms(userId);

            if (!visitedFilms.Any())
            {
                return Enumerable.Empty<Film>();
            }

            // 2. Get similarities for the user's films
            var filmIds = visitedFilms.Select(f => f.Id).ToList();

            var similarities = _unitOfWork.Repository<FilmSimilarity>().Get(
                filter: fs => filmIds.Contains(fs.Film1Id) || filmIds.Contains(fs.Film2Id),
                includeProperties: "Film1,Film2"
            ).ToList();

            // 3. Calculate the average similarity for each candidate film
            var recommendedFilms = new Dictionary<Film, double>();

            foreach (var similarity in similarities)
            {
                // Determine which film is the candidate
                var candidateFilm = filmIds.Contains(similarity.Film1Id) ? similarity.Film2 : similarity.Film1;

                if (!visitedFilms.Contains(candidateFilm))
                {
                    if (!recommendedFilms.ContainsKey(candidateFilm))
                    {
                        recommendedFilms[candidateFilm] = 0;
                    }

                    recommendedFilms[candidateFilm] += similarity.SimilarityScore;
                }
            }

            // Average similarity for each candidate film
            foreach (var film in recommendedFilms.Keys.ToList())
            {
                recommendedFilms[film] /= visitedFilms.Count();
            }

            // 4. Filter by similarity threshold and return the result
            const double similarityThreshold = 0.5;

            return recommendedFilms
                .Where(r => r.Value >= similarityThreshold)
                .OrderByDescending(r => r.Value)
                .Select(r => r.Key)
                .ToList();
        }

        private IEnumerable<Film> GetVisitedFilms(int userId)
        {
            var bookings = _unitOfWork.Repository<Booking>().Get(
                filter: b => (b.Status.Name == "Checked-in" || b.Status.Name == "Reserved") && b.UserId == userId,
                includeProperties: "Session"
            );

            return bookings.Select(b => b.Session.Film).Distinct().ToList();
        }
    }

}
