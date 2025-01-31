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
        // Отримання відфільтрованих сеансів через сервісний шар
        var sessions = await _sessionService.GetFilteredSessionsAsync(filter);

        // Вибираємо унікальні фільми зі списку сеансів
        var uniqueFilms = sessions
            .Select(s => s.Film)
            .Distinct()
            .ToList();
        
        // Виклик сервісів для отримання жанрів та вікових рейтингів
        filter.AvailableGenres = (await _sessionService.GetAllGenresAsync()).ToList();
        filter.AvailableAgeRatings = (await _sessionService.GetAllAgeRatingsAsync()).ToList();
        filter.Films = uniqueFilms;
        return View(filter);
    }
}