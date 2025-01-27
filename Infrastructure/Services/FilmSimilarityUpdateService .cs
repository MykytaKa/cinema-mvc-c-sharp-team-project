using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities;

namespace Core.Interfaces.Services
{
    public class FilmSimilarityUpdateService : IFilmSimilarityUpdateService
    {
        private readonly IUnitOfWork _unitOfWork;

        public FilmSimilarityUpdateService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task UpdateSimilaritiesForFilmAsync(int filmId)
        {
            // 1. Get the film from the database along with its genres and actors
            var film = await _unitOfWork.Repository<Film>().GetByIdAsync(filmId, includeProperties: "Genres,Actors");

            if (film == null)
                throw new ArgumentException($"Film with ID {filmId} does not exist.");

            // 2. Get all other films from the database
            var otherFilms = await _unitOfWork.Repository<Film>().GetAsync(
                filter: f => f.Id != filmId,
                includeProperties: "Genres,Actors"
            );

            // 3. Calculate similarities between the new film and the other films
            var similarities = otherFilms.Select(otherFilm => new FilmSimilarity
            {
                Film1Id = filmId,
                Film2Id = otherFilm.Id,
                SimilarityScore = CalculateCosineSimilarity(film, otherFilm)
            }).ToList();

            // 4. Remove old similarity records for this film
            var existingSimilarities = await _unitOfWork.Repository<FilmSimilarity>().GetAsync(
                filter: fs => fs.Film1Id == filmId || fs.Film2Id == filmId
            );

            await _unitOfWork.Repository<FilmSimilarity>().DeleteRangeAsync(existingSimilarities);

            // 5. Add new similarity records
            await _unitOfWork.Repository<FilmSimilarity>().AddRangeAsync(similarities);

            // 6. Save the changes
            await _unitOfWork.SaveAsync();
        }

        private double CalculateCosineSimilarity(Film film1, Film film2)
        {
            var genres1 = film1.Genres.Select(g => g.Id).ToHashSet();
            var genres2 = film2.Genres.Select(g => g.Id).ToHashSet();

            var actors1 = film1.Actors.Select(a => a.Id).ToHashSet();
            var actors2 = film2.Actors.Select(a => a.Id).ToHashSet();

            // Determine the intersection of genres and actors
            var intersectionGenres = genres1.Intersect(genres2).Count();
            var intersectionActors = actors1.Intersect(actors2).Count();

            // Normalization for calculating cosine similarity
            var norm1 = Math.Sqrt(genres1.Count + actors1.Count);
            var norm2 = Math.Sqrt(genres2.Count + actors2.Count);

            if (norm1 == 0 || norm2 == 0) return 0;

            return (intersectionGenres + intersectionActors) / (norm1 * norm2);
        }
    }

}
