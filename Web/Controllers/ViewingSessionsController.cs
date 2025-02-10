using Application.Interfaces;
using Core.FiltersModels;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

public class ViewingSessionsController(ISessionService sessionService) : Controller
{
    public async Task<IActionResult> ViewingSessions(SessionFilterModel filter)
    {
        // Виклик сервісів
        filter = await sessionService.GetFilteredFilmsAsync(filter);
        return View(filter);
    }
}