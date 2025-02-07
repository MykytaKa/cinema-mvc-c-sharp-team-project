using Application.Interfaces;
using Core.FiltersModels;
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
        // Виклик сервісів
        filter.AvailableGenres = (await _sessionService.GetAllGenresAsync()).ToList();
        filter.AvailableAgeRatings = (await _sessionService.GetAllAgeRatingsAsync()).ToList();
        filter = await _sessionService.GetFilteredFilmsAsync(filter);
        return View(filter);
    }
}