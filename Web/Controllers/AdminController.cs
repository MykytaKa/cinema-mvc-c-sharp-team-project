using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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



        //Session 

        public async Task<IActionResult> AddSession(int page = 1, int pageSize = 10)
        {
            var films = await _unitOfWork.Repository<Film>().GetAllAsync();
            var halls = await _unitOfWork.Repository<Hall>().GetAllAsync();
            var allSessions = await _unitOfWork.Repository<Session>().GetAllAsync();

            int totalSessions = allSessions.Count();
            var model = new SessionViewModel
            {
                AllFilms = films.ToList(),
                AllHalls = halls.ToList(),
                AllSessions = allSessions
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList(),
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalSessions / (double)pageSize)
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddSessionToDB([FromForm] SessionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AllFilms = (await _unitOfWork.Repository<Film>().GetAllAsync()).ToList();
                model.AllHalls = (await _unitOfWork.Repository<Hall>().GetAllAsync()).ToList();
                model.AllSessions = (await _unitOfWork.Repository<Session>().GetAllAsync()).ToList();
                return View("AddSession", model);
            }

            try
            {
                var film = await _unitOfWork.Repository<Film>().GetByIDAsync(model.FilmId);
                var hall = await _unitOfWork.Repository<Hall>().GetByIDAsync(model.HallId);

                if (film == null || hall == null)
                {
                    TempData["Error"] = "Invalid Film or Hall selection.";
                    return RedirectToAction("AddSession");
                }

                var allSessions = await _unitOfWork.Repository<Session>().GetAllAsync();

                var sessions = new List<Session>();
                for (int i = 0; i < model.NumberOfDays; i++)
                {
                    var sessionDateTimeBeg = model.DateTimeBeg.AddDays(i);
                    var sessionDateTimeEnd = sessionDateTimeBeg.Add(film.Duration).AddMinutes(5); 

                    bool isConflict = allSessions.Any(s =>
                        s.HallId == model.HallId &&
                        ((sessionDateTimeBeg >= s.DateTimeBeg && sessionDateTimeBeg < s.DateTimeEnd) ||
                         (sessionDateTimeEnd > s.DateTimeBeg && sessionDateTimeEnd <= s.DateTimeEnd) ||
                         (sessionDateTimeBeg <= s.DateTimeBeg && sessionDateTimeEnd >= s.DateTimeEnd)));

                    if (isConflict)
                    {
                        TempData["Error"] = $"Conflict detected: Another session is already scheduled in Hall {hall.Name} on {sessionDateTimeBeg}.";
                        return RedirectToAction("AddSession");
                    }

                    var session = new Session
                    {
                        DateTimeBeg = sessionDateTimeBeg,
                        DateTimeEnd = sessionDateTimeEnd,
                        Price = model.Price,
                        FilmId = model.FilmId,
                        HallId = model.HallId
                    };
                    sessions.Add(session);
                }

                await _unitOfWork.Repository<Session>().InsertRangeAsync(sessions);
                await _unitOfWork.SaveAsync();

                TempData["Success"] = "Sessions successfully added!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding sessions.");
                TempData["Error"] = "An error occurred while adding the sessions. Please try again.";
            }

            return RedirectToAction("AddSession");
        }

        public async Task<IActionResult> ChangeSession(int page = 1, int pageSize = 10)
        {
            // Завантажуємо всі сесії, фільми та зали
            var allSessions = await _unitOfWork.Repository<Session>()
                .GetAllAsync(s => s.Include(session => session.Film)
                                   .Include(session => session.Hall));
            var allFilms = await _unitOfWork.Repository<Film>().GetAllAsync();
            var allHalls = await _unitOfWork.Repository<Hall>().GetAllAsync();

            int totalSessions = allSessions.Count();
            var model = new SessionViewModel
            {
                AllSessions = allSessions
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList(),
                AllFilms = allFilms.ToList(),
                AllHalls = allHalls.ToList(),
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalSessions / (double)pageSize)
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSession(int SessionId, int FilmId, int HallId, DateTime DateTimeBeg, decimal? Price)
        {
            try
            {
                var session = await _unitOfWork.Repository<Session>().GetByIDAsync(SessionId);
                if (session == null)
                {
                    TempData["Error"] = "Session not found.";
                    return RedirectToAction("ChangeSession");
                }

                var film = await _unitOfWork.Repository<Film>().GetByIDAsync(FilmId);
                if (film == null)
                {
                    TempData["Error"] = "Film not found.";
                    return RedirectToAction("ChangeSession");
                }

                var sessionDateTimeEnd = DateTimeBeg.AddMinutes(film.Duration.TotalMinutes + 5); 

                var allSessions = await _unitOfWork.Repository<Session>().GetAllAsync(s =>
                    s.Include(session => session.Film)
                     .Include(session => session.Hall)
                );

                var conflictSession = allSessions
                    .Where(s => s.HallId == HallId &&
                                s.Id != SessionId &&
                                (
                                    (DateTimeBeg >= s.DateTimeBeg && DateTimeBeg < s.DateTimeEnd) ||
                                    (sessionDateTimeEnd > s.DateTimeBeg && sessionDateTimeEnd <= s.DateTimeEnd) || 
                                    (DateTimeBeg <= s.DateTimeBeg && sessionDateTimeEnd >= s.DateTimeEnd) 
                                )
                    ).ToList();

                if (conflictSession.Any())
                {
                    TempData["Error"] = $"Conflict detected: Another session is already scheduled in {session.Hall.Name} during the selected time.";
                    return RedirectToAction("ChangeSession");
                }

                session.FilmId = FilmId;
                session.HallId = HallId;
                session.DateTimeBeg = DateTimeBeg;
                session.DateTimeEnd = sessionDateTimeEnd;

                if (Price.HasValue && session.Price != Price.Value)
                {
                    session.Price = Price.Value;
                }

                _unitOfWork.Repository<Session>().UpdateAsync(session);
                await _unitOfWork.SaveAsync();

                TempData["Success"] = "Session updated successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating the session.");
                TempData["Error"] = "An error occurred while updating the session. Please try again.";
            }

            return RedirectToAction("ChangeSession");
        }

        //Film

        public async Task<IActionResult> AddFilm(int page = 1, int pageSize = 10)
        {
            var model = new FilmViewModel();

            var allFilms = await _unitOfWork.Repository<Film>().GetAllAsync();
            var allActors = await _unitOfWork.Repository<Actor>().GetAllAsync();
            var allGenres = await _unitOfWork.Repository<Genre>().GetAllAsync();

            model.AllActors = allActors.ToList();
            model.AllGenres = allGenres.ToList();

            int totalFilms = allFilms.Count();
            model.TotalPages = (int)Math.Ceiling(totalFilms / (double)pageSize);
            model.CurrentPage = page;

            model.AllFilms = allFilms
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

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
        
        //Actor

        public IActionResult AddActor(int page = 1, int pageSize = 10)
        {
            var model = new ActorViewModel();

            var allActors = _unitOfWork.Repository<Actor>().GetAllAsync().Result.ToList();

            int totalActors = allActors.Count;
            model.TotalPages = (int)Math.Ceiling(totalActors / (double)pageSize);
            model.CurrentPage = page;

            model.AllActors = allActors
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return View(model);
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
