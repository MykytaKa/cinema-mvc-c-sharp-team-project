using Application.Interfaces;
using Core.Entities;
using Microsoft.Extensions.Logging;


namespace Application.Services
{
    public class RecommendationService : IRecommendationService
    {
        private readonly ILogger<RecommendationService> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public RecommendationService(IUnitOfWork unitOfWork, ILogger<RecommendationService> logger)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Film>> GetRecommendations(int userId)
        {
            _logger.LogInformation("Fetching recommendations for user with ID {UserId}.", userId);

            // 1. Get all films the user has visited
            _logger.LogInformation("Retrieving films visited by user with ID {UserId}.", userId);
            var visitedFilms = await GetVisitedFilms(userId);

            if (!visitedFilms.Any())
            {
                _logger.LogInformation("User with ID {UserId} has not visited any films. No recommendations available.", userId);
                return Enumerable.Empty<Film>();
            }

            _logger.LogInformation("User with ID {UserId} has visited {VisitedFilmCount} films.", userId, visitedFilms.Count());

            // 2. Get similarities for the user's films
            var filmIds = visitedFilms.Select(f => f.Id).ToList();
            _logger.LogInformation("Fetching similarities for visited films with IDs: {FilmIds}.", string.Join(", ", filmIds));

            var similarities = await _unitOfWork.Repository<FilmSimilarity>().GetAsync(
                filter: fs => filmIds.Contains(fs.Film1Id) || filmIds.Contains(fs.Film2Id),
                includeProperties: "Film1,Film2"
            );

            // 3. Calculate the average similarity for each candidate film
            var recommendedFilms = new Dictionary<Film, double>();

            _logger.LogInformation("Calculating recommendations based on similarities.");
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
            const double similarityThreshold = 0.14;
            _logger.LogInformation("Filtering films with similarity above {SimilarityThreshold}.", similarityThreshold);

            var filteredRecommendations = recommendedFilms
                .Where(r => r.Value >= similarityThreshold)
                .OrderByDescending(r => r.Value)
                .Select(r => r.Key)
                .ToList();

            _logger.LogInformation("Found {RecommendedFilmCount} films above the similarity threshold for user with ID {UserId}.", filteredRecommendations.Count(), userId);

            return filteredRecommendations;
        }

        private async Task<IEnumerable<Film>> GetVisitedFilms(int userId)
        {
            _logger.LogInformation("Retrieving bookings for user with ID {UserId}.", userId);
            var bookings = await _unitOfWork.Repository<Booking>().GetAsync(
                filter: b => (b.Status.Name == "Checked-in" || b.Status.Name == "Reserved") && b.UserId == userId,
                includeProperties: "Session.Film"
            );

            _logger.LogInformation("Found {BookingCount} bookings for user with ID {UserId}.", bookings.Count(), userId);
            return bookings
                .Where(b => b.Session?.Film != null)
                .Select(b => b.Session.Film!)
                .Distinct();
        }
    }


}
