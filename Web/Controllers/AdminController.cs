using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Web.Models;

namespace Web.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public AdminController(ILogger<AdminController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Admin()
        {
            return View();
        }

        public IActionResult AddActor()
        {
            return View();
        }

        public async Task<IActionResult> AddFilm()
        {
            var model = new FilmViewModel();
            var actors = await _unitOfWork.Repository<Actor>().GetAllAsync();
            var genres = await _unitOfWork.Repository<Genre>().GetAllAsync();

            model.AllActors = actors.ToList();
            model.AllGenres = genres.ToList();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddFilmToDB([FromForm] FilmViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AllActors = (await _unitOfWork.Repository<Actor>().GetAllAsync()).ToList();
                model.AllGenres = (await _unitOfWork.Repository<Genre>().GetAllAsync()).ToList();
                return View("AddFilm", model);
            }

            var film = new Film
            {
                Name = model.Name,
                PosterURL = model.PosterURL,
                TrailerURL = model.TrailerURL,
                Description = model.Description,
                Rating = model.Rating,
                ReleaseRate = model.ReleaseRate,
                Duration = TimeSpan.Parse(model.Duration),
                AgeRating = model.AgeRating,
                Director = model.Director,
                Actors = new List<Actor>(),
                Genres = new List<Genre>()
            };

            try
            {
                await _unitOfWork.Repository<Film>().InsertAsync(film);
                await _unitOfWork.SaveAsync();
                if (model.SelectedActorIds != null && model.SelectedActorIds.Any())
                {
                    foreach (var actorId in model.SelectedActorIds)
                    {
                        var actor = await _unitOfWork.Repository<Actor>().GetByIDAsync(actorId);
                        if (actor != null)
                        {
                            film.Actors.Add(actor);
                            _logger.LogInformation("Added actor with ID {Id} to film.", actorId);
                        }
                        else
                        {
                            _logger.LogWarning("Actor with ID {Id} not found.", actorId);
                        }
                    }
                }
                else
                {
                    _logger.LogWarning("No actors selected for the film.");
                }
                if (model.SelectedGenreIds != null && model.SelectedGenreIds.Any())
                {
                    foreach (var genreId in model.SelectedGenreIds)
                    {
                        var genre = await _unitOfWork.Repository<Genre>().GetByIDAsync(genreId);
                        if (genre != null)
                        {
                            film.Genres.Add(genre);
                            _logger.LogInformation("Added genre with ID {Id} to film.", genreId);
                        }
                        else
                        {
                            _logger.LogWarning("Genre with ID {Id} not found.", genreId);
                        }
                    }
                }
                else
                {
                    _logger.LogWarning("No genres selected for the film.");
                }

                await _unitOfWork.SaveAsync();
                TempData["Success"] = "Film successfully added!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding film to the database");
                TempData["Error"] = "An error occurred while adding the film. Please try again.";
            }

            return RedirectToAction("AddFilm");
        }

    
        [HttpPost]
        public async Task<IActionResult> AddActorToDB([FromForm] ActorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("AddActor", model);
            }

            var actor = new Actor
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
            };

            try
            {
                await _unitOfWork.Repository<Actor>().InsertAsync(actor);
                await _unitOfWork.SaveAsync();
                TempData["Success"] = "Actor successfully added!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding actor to the database");
                TempData["Error"] = "An error occurred while adding the actor. Please try again.";
            }

            return RedirectToAction("AddActor");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
