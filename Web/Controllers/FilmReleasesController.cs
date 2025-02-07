using Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Web.Models;

namespace Web.Controllers
{
    public class FilmReleasesController : Controller
    {
        private readonly IReleasesService _releaseService;
        private readonly ILogger<FilmReleasesController> _logger;

        public FilmReleasesController(IReleasesService releaseService, ILogger<FilmReleasesController> logger)
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

            DateTime currentDate = new DateTime(year, month, 1);
            DateTime now = DateTime.Now;

            DateTime? prevMonth = currentDate > now ? currentDate.AddMonths(-1) : (DateTime?)null;
            DateTime nextMonth = currentDate.AddMonths(1);

            var newReleasesDTOs = await _releaseService.GetNewReleases(month, year);

            return View(new NewReleasesViewModel
            {
                Month = month,
                Year = year,
                FilmDTOs = newReleasesDTOs.ToList(),
                PrevMonth = prevMonth?.Month,
                PrevYear = prevMonth?.Year,
                NextMonth = nextMonth.Month,
                NextYear = nextMonth.Year
            });
        }
    }
}
