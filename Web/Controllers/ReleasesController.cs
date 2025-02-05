using Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Web.Models;

namespace Web.Controllers
{
    public class ReleasesController : Controller
    {
        private readonly IReleasesService _releaseService;
        private readonly ILogger<ReleasesController> _logger;

        public ReleasesController(IReleasesService releaseService, ILogger<ReleasesController> logger)
        {
            _releaseService = releaseService;
            _logger = logger;
        }

        public async Task<IActionResult> Releases(int month = 0, int year = 0)
        {
            if (month == 0 || year == 0)
            {
                month = DateTime.Now.Month;
                year = DateTime.Now.Year;
            }

            var newReleasesDTOs = await _releaseService.GetNewReleases(month, year);

            return View(new NewReleasesViewModel
            {
                Month = month,
                Year = year,
                FilmDTOs = newReleasesDTOs.ToList()
            });
        }
    }
}
