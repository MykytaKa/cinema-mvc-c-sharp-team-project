using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using Web.Models;

namespace Web.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly string _apiKey = "c8a260e94876a3a04f0317efa68269ac";
        private readonly IConfiguration _configuration;

        public AdminController(ILogger<AdminController> logger, IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
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

        public async Task<IActionResult> DeleteSession(int page = 1, int pageSize = 10)
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
        public async Task<IActionResult> DeleteSessionById(int sessionId)
        {
            var sessionRepo = _unitOfWork.Repository<Session>();
            var session = await sessionRepo.GetByIDAsync(sessionId);

            if (session != null)
            {
                sessionRepo.DeleteAsync(session);
                await _unitOfWork.SaveAsync();
            }

            return RedirectToAction("DeleteSession");
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

        public async Task <IActionResult> DeleteFilm(int page=1, int pageSize = 10)
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
        public async Task<IActionResult> DeleteFilmById(int filmId)
        {
            var film = await _unitOfWork.Repository<Film>().GetByIDAsync(filmId);
            if (film != null)
            {
                _unitOfWork.Repository<Film>().DeleteAsync(film);
                await _unitOfWork.SaveAsync();
            }
            return RedirectToAction(nameof(DeleteFilm));
        }

        [HttpPost]
        public async Task<IActionResult> AddFilmToDB([FromBody] FilmViewModel model)
        {
          
            if (model == null)
            {
                return BadRequest("Invalid data.");
            }

        
            var durationString = model.Duration; 
            var duration = ConvertDurationToTimeSpan(durationString); 

      
            var actorEntities = new List<Actor>();
            foreach (var actorName in model.Actors.Split(','))
            {
                var actor = actorName.Trim();
                var nameParts = actor.Split(' ');

                string firstName = nameParts[0];
                string lastName = nameParts.Length > 1 ? string.Join(" ", nameParts.Skip(1)) : string.Empty;

                var existingActor = await _unitOfWork.Repository<Actor>()
                    .GetFirstOrDefaultAsync(a => a.FirstName == firstName && a.LastName == lastName);

                if (existingActor == null)
                {
                    var newActor = new Actor
                    {
                        FirstName = firstName,
                        LastName = lastName
                    };
                    await _unitOfWork.Repository<Actor>().InsertAsync(newActor);
                    actorEntities.Add(newActor);
                }
                else
                {
                    actorEntities.Add(existingActor);
                }
            }

            var genreEntities = new List<Genre>();
            foreach (var genreName in model.Genres.Split(','))
            {
                var genre = genreName.Trim();
                var existingGenre = await _unitOfWork.Repository<Genre>()
                    .GetFirstOrDefaultAsync(g => g.Name == genre);

                if (existingGenre == null)
                {
                    var newGenre = new Genre { Name = genre };
                    await _unitOfWork.Repository<Genre>().InsertAsync(newGenre);
                    genreEntities.Add(newGenre);
                }
                else
                {
                    genreEntities.Add(existingGenre);
                }
            }

            var filmEntity = new Film
            {
                Name = model.Name,
                PosterURL = model.PosterURL,
                TrailerURL = model.TrailerURL,
                Description = model.Description,
                Rating = model.Rating,
                ReleaseRate = model.ReleaseRate,
                Duration = duration, 
                AgeRating = model.AgeRating,
                Director = model.Director,
                Actors = actorEntities,
                Genres = genreEntities
            };

            await _unitOfWork.Repository<Film>().InsertAsync(filmEntity);

            await _unitOfWork.SaveAsync();

            return Ok(new { message = "Film added successfully!" });
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
        

        //Span
        private TimeSpan ConvertDurationToTimeSpan(string duration)
        {
            var minutes = int.Parse(duration.Replace(" minutes", "").Trim());

            return TimeSpan.FromMinutes(minutes);
        }

        public IActionResult GetApiKey()
        {
            var apiKey = _configuration["TMDbApiKey"];
            return Json(new { apiKey });
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
