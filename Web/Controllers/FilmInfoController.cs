using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class FilmInfoController : Controller
    {
        private readonly IFilmInfoService _filminfo;

        public FilmInfoController(IFilmInfoService filminfo)
        {
            _filminfo = filminfo;
        }

        public async Task<IActionResult> FilmInfo(int id)
        {
            var film = await _filminfo.FilmInfoAsync(id);
            if (film == null)
            {
                return NotFound(); 
            }

            return View(film);
        }
    }
}