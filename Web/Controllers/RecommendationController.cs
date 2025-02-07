
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Models;

namespace Web.Controllers
{
    public class RecommendationController: Controller
    {
        private readonly ILogger<RecommendationController> _logger;
        private readonly IRecommendationService _recommendationService;

        public RecommendationController(
            ILogger<RecommendationController> logger,
            IRecommendationService recoService
            )
        {
            _logger = logger;
            _recommendationService = recoService;
        }

        [Authorize]
        public async Task<IActionResult> Recommend()
        {
            _logger.LogInformation("Starting recommendation process.");

            try
            {
                var userIdClaim = User.FindFirst("UserId")?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    _logger.LogWarning("Failed to retrieve a valid User ID from token.");
                    return Unauthorized("Invalid User ID.");
                }

                _logger.LogDebug("Fetching recommended films...");
                var recoFilms = await _recommendationService.GetRecommendations(userId);

                if (recoFilms == null || !recoFilms.Any())
                {
                    _logger.LogWarning("No recommended films were found.");
                    return View(new List<FilmRecoViewModel>());
                }

                _logger.LogDebug("Mapping recommended films to view models...");
                var recoFilmModels = recoFilms.Select(f => new FilmRecoViewModel
                {
                    Title = f.Name,
                    PosterUrl = f.PosterURL
                }).ToList();

                _logger.LogInformation("Successfully generated recommended films list.");
                return View(recoFilmModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while generating film recommendations.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
