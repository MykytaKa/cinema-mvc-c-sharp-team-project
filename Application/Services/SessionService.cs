using Core.Entities;
using Core.FiltersModels;
using Application.Interfaces;
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
        var sessions = await _unitOfWork.Repository<Session>()
            .GetAsync(includeProperties: "Film.Genres,Hall,Bookings");
        
        
        // Фільтрація за назвою фільму (пошук часткового збігу)
        if (!string.IsNullOrEmpty(filter.FilmName))
        {
            sessions = sessions.Where(s => s.Film.Name.Contains(filter.FilmName, StringComparison.OrdinalIgnoreCase));
        }
        
        // Фільтрує одразу встановлюючи сьогоднішню дату
        if (!filter.SessionDate.HasValue)
        {
            filter.SessionDate = DateTime.Today;
        }
        
        // Фільтрація за датою
        sessions = sessions.Where(s => s.DateTimeBeg.Date == filter.SessionDate.Value.Date);
        
        // Фільтрація за часом сеансу (опціонально)
        var now = DateTime.Now;
        
        if (filter.SessionDate != null && filter.SessionDate.Value.Date == now.Date)
        {
            sessions = sessions.Where(s => s.DateTimeEnd > now);
        }
        
        if (filter.SessionTime.HasValue)
        {
            sessions = sessions.Where(s => s.DateTimeBeg.TimeOfDay == filter.SessionTime.Value);
        }

        // Фільтрація за жанром
        if (!string.IsNullOrEmpty(filter.Genre))
        {
            sessions = sessions.Where(s => s.Film.Genres.Any(g => g.Name == filter.Genre));
        }

        // Фільтрація за рейтингом фільму
        if (filter.MinRating.HasValue)
        {
            sessions = sessions.Where(s => s.Film.Rating >= (decimal)filter.MinRating.Value);
        }
        if (filter.MaxRating.HasValue)
        {
            sessions = sessions.Where(s => s.Film.Rating <= (decimal)filter.MaxRating.Value);
        }

        // Фільтрація за віковим рейтингом
        if (!string.IsNullOrEmpty(filter.AgeRating))
        {
            sessions = sessions.Where(s => s.Film.AgeRating == filter.AgeRating);
        }

        // Фільтрація за датою виходу фільму
        if (filter.ReleaseDate.HasValue)
        {
            sessions = sessions.Where(s => s.Film.ReleaseRate.Date == filter.ReleaseDate.Value.Date);
        }

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
        // Фільтруємо сеанси через репозиторій
        var sessions = await GetFilteredSessionsAsync(filter);

        // Групуємо фільми та додаємо до них тільки відфільтровані сеанси
        var uniqueFilms = sessions
            .GroupBy(s => s.Film.Id)
            .Select(g =>
            {
                var film = g.First().Film;
                film.Sessions = g.ToList(); // Додаємо тільки відфільтровані сеанси
                return film;
            })
            .ToList();

        // Заповнюємо фільтр доступними жанрами та віковими рейтингами
        filter.AvailableGenres = (await GetAllGenresAsync()).ToList();
        filter.AvailableAgeRatings = (await GetAllAgeRatingsAsync()).ToList();
        filter.Films = uniqueFilms;

        return filter;
    }
}