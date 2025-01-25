

using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

public class ViewingSessionsController : Controller
{
    private readonly ILogger<ViewingSessionsController> _logger;

    public ViewingSessionsController(ILogger<ViewingSessionsController> logger)
    {
        _logger = logger;
    }

    public IActionResult ViewingSessions()
    {
        return View();
    }
}