using Application.Interfaces;
using Core.Entities;
using Core.FiltersModels;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class SessionService : ISessionService
{
    private readonly IUnitOfWork _unitOfWork;

    public SessionService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<IEnumerable<Session>> GetFilteredSessionsAsync(SessionFilterModel filter)
    {
        //робить всю фільтрацію ще в БД (EF Core), а не в пам’яті.
        var query = _unitOfWork.Repository<Session>().GetQueryable()
            .Include(s => s.Film)
                .ThenInclude(f => f.Genres)
            .Include(s => s.Hall)
            .Include(s => s.Bookings)
            .AsQueryable();
        
        // Фільтрація за назвою фільму (пошук часткового збігу)
        if (!string.IsNullOrEmpty(filter.FilmName))
        {
            query = query.Where(s => s.Film.Name.Contains(filter.FilmName, StringComparison.OrdinalIgnoreCase));
        } 
        
        // Фільтрація за датою
        filter.SessionDate ??= DateTime.Today;
        query = query.Where(s => s.DateTimeBeg.Date == filter.SessionDate.Value.Date);
        var now = DateTime.Now;
        if (filter.SessionDate == now.Date)
        {
            query = query.Where(s => s.DateTimeEnd > now);
        }
        
        // Фільтрація за часом сеансу
        if (filter.SessionTime.HasValue)
        {
            query = query.Where(s => s.DateTimeBeg.TimeOfDay == filter.SessionTime.Value);
        }

        // Фільтрація за жанром
        if (!string.IsNullOrEmpty(filter.Genre))
        {
            query = query.Where(s => s.Film.Genres.Any(g => g.Name == filter.Genre));
        }

        // Фільтрація за рейтингом фільму
        if (filter.MinRating.HasValue)
        {
            query = query.Where(s => s.Film.Rating >= (decimal)filter.MinRating.Value);
        }
        if (filter.MaxRating.HasValue)
        {
            query = query.Where(s => s.Film.Rating <= (decimal)filter.MaxRating.Value);
        }

        // Фільтрація за віковим рейтингом
        if (!string.IsNullOrEmpty(filter.AgeRating))
        {
            query = query.Where(s => s.Film.AgeRating == filter.AgeRating);
        }

        // Фільтрація за датою виходу фільму
        if (filter.ReleaseDate.HasValue)
        {
            query = query.Where(s => s.Film.ReleaseRate.Date == filter.ReleaseDate.Value.Date);
        }

        var sessions = await query.ToListAsync();

        return sessions;
    }
    
    public async Task<IEnumerable<string>> GetAllGenresAsync()
    {
        var films = await _unitOfWork.Repository<Film>().GetAsync(includeProperties: "Genres");
        return films
            .SelectMany(f => f.Genres.Select(g => g.Name))
            .Distinct()
            .OrderBy(g => g)
            .ToList();
    }

    public async Task<IEnumerable<string>> GetAllAgeRatingsAsync()
    {
        var films = await _unitOfWork.Repository<Film>().GetAllAsync();
        return films
            .Select(f => f.AgeRating)
            .Distinct()
            .OrderBy(r => r)
            .ToList();
    }

    public async Task<SessionFilterModel> GetFilteredFilmsAsync(SessionFilterModel filter)
    {
        // Фільтруємо сеанси
        var sessions = await GetFilteredSessionsAsync(filter);

        // Групуємо фільми та додаємо до них тільки відфільтровані сеанси
        var uniqueFilms = sessions
            .GroupBy(s => s.Film.Id)
            .Select(g =>
            {
                var film = g.First().Film;
                return new Film
                {
                    Id = film.Id,
                    Name = film.Name,
                    PosterURL = film.PosterURL,
                    Rating = film.Rating,
                    AgeRating = film.AgeRating,
                    Duration = film.Duration,
                    ReleaseRate = film.ReleaseRate,
                    Genres = film.Genres,
                    Sessions = g.ToList()
                };
            })
            .ToList();

        // Заповнюємо фільтр доступними жанрами та віковими рейтингами
        filter.AvailableGenres = (await GetAllGenresAsync()).ToList();
        filter.AvailableAgeRatings = (await GetAllAgeRatingsAsync()).ToList();
        filter.Films = uniqueFilms;

        return filter;
    }
}