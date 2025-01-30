using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Core.Interfaces.Services
{
    public class FilmSimilarityUpdateService : IFilmSimilarityUpdateService
    {
        private readonly ILogger<FilmSimilarityUpdateService> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public FilmSimilarityUpdateService(IUnitOfWork unitOfWork, ILogger<FilmSimilarityUpdateService> logger)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task UpdateSimilaritiesForFilmAsync(int filmId)
        {
            _logger.LogInformation("Starting similarity update for film with ID {FilmId}.", filmId);

            // 1. Get the film from the database along with its genres and actors
            var film = await _unitOfWork.Repository<Film>().GetByIDAsync(filmId, includeProperties: "Genres,Actors");

            if (film == null)
            {
                _logger.LogError("Film with ID {FilmId} does not exist.", filmId);
                throw new ArgumentException($"Film with ID {filmId} does not exist.");
            }

            _logger.LogInformation("Retrieved film with ID {FilmId}.", filmId);

            // 2. Get all other films from the database
            _logger.LogInformation("Fetching other films from the database.");
            var otherFilms = await _unitOfWork.Repository<Film>().GetAsync(
                filter: f => f.Id != filmId
            );

            // 3. Calculate similarities between the new film and the other films
            _logger.LogInformation("Calculating similarities for {FilmCount} films.", otherFilms.Count());
            var otherFilmsWithGenres = await _unitOfWork.Repository<Film>().GetAllAsync(query => 
                query.Where(f => f.Id != filmId).Include(f => f.Genres).Include(f => f.Actors));

            var similarities = new List<FilmSimilarity>();

            foreach (var otherFilm in otherFilmsWithGenres)
            {
                var similarityScore = CalculateCosineSimilarity(film, otherFilm);
                similarities.Add(new FilmSimilarity
                {
                    Film1Id = filmId,
                    Film2Id = otherFilm.Id,
                    SimilarityScore = similarityScore
                });
            }

            // 4. Remove old similarity records for this film
            _logger.LogInformation("Fetching existing similarities for film with ID {FilmId}.", filmId);
            var existingSimilarities = await _unitOfWork.Repository<FilmSimilarity>().GetAsync(
                filter: fs => fs.Film1Id == filmId || fs.Film2Id == filmId
            );

            _logger.LogInformation("Found {ExistingSimilarityCount} existing similarities.", existingSimilarities.Count());
            foreach (var similarity in existingSimilarities)
            {
                await _unitOfWork.Repository<FilmSimilarity>().DeleteAsync(similarity);
            }

            // 5. Add new similarity records
            _logger.LogInformation("Inserting {NewSimilarityCount} new similarities.", similarities.Count());
            await _unitOfWork.Repository<FilmSimilarity>().InsertRangeAsync(similarities);

            // 6. Save the changes
            _logger.LogInformation("Saving changes to the database.");
            await _unitOfWork.SaveAsync();

            _logger.LogInformation("Film similarity update for film with ID {FilmId} completed successfully.", filmId);
        }

        private double CalculateCosineSimilarity(Film film1, Film film2)
        {
            _logger.LogDebug("Calculating cosine similarity between film ID {Film1Id} and film ID {Film2Id}.", film1.Id, film2.Id);

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

            if (norm1 == 0 || norm2 == 0)
            {
                _logger.LogWarning("One or both films have no genres or actors, returning 0 similarity.");
                return 0;
            }

            var similarity = (intersectionGenres + intersectionActors) / (norm1 * norm2);
            _logger.LogDebug("Calculated similarity score: {SimilarityScore}.", similarity);

            return similarity;
        }

        public async Task DeleteFilmWithSimilaritiesAsync(int filmId)
        {
            _logger.LogInformation("Deleting film with ID {FilmId} and its similarity records.", filmId);

            // Delete every record which contains the film
            var similarities = await _unitOfWork.Repository<FilmSimilarity>().GetAsync(
                filter: fs => fs.Film1Id == filmId || fs.Film2Id == filmId
            );

            _logger.LogInformation("Found {Count} similarity records to delete.", similarities.Count());

            foreach (var similarity in similarities)
            {
                await _unitOfWork.Repository<FilmSimilarity>().DeleteAsync(similarity);
            }

            // Delete the film
            var film = await _unitOfWork.Repository<Film>().GetByIDAsync(filmId);
            if (film == null)
            {
                _logger.LogError("Film with ID {FilmId} not found.", filmId);
                throw new ArgumentException($"Film with ID {filmId} not found.");
            }

            await _unitOfWork.Repository<Film>().DeleteAsync(film);

            // Save changes
            await _unitOfWork.SaveAsync();
            _logger.LogInformation("Successfully deleted film with ID {FilmId} and its similarity records.", filmId);
        }

    }
}
