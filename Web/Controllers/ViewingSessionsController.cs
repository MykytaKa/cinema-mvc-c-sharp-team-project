using Core.FiltersModels;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

public class ViewingSessionsController : Controller
{
    private readonly ISessionService _sessionService;

    public ViewingSessionsController(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    public async Task<IActionResult> ViewingSessions(SessionFilterModel filter)
    {
        if (filter.SessionDate == null)
        {
            filter.SessionDate = DateTime.Today;
        }
        // Отримання відфільтрованих сеансів через сервісний шар
        var sessions = await _sessionService.GetFilteredSessionsAsync(filter);

        // Вибираємо унікальні фільми зі списку сеансів
        var uniqueFilms = sessions
            .GroupBy(s => s.Film.Id)
            .Select(g =>
            {
                var film = g.First().Film;
                film.Sessions = g.ToList(); // Додаємо тільки відфільтровані сеанси
                return film;
            })
            .ToList();
        
        // Виклик сервісів для отримання жанрів та вікових рейтингів
        filter.AvailableGenres = (await _sessionService.GetAllGenresAsync()).ToList();
        filter.AvailableAgeRatings = (await _sessionService.GetAllAgeRatingsAsync()).ToList();
        filter.Films = uniqueFilms;
        return View(filter);
    }
}