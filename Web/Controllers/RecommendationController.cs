using Core.Entities;
using Core.Interfaces;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class RecommendationController: Controller
    {
        private readonly ILogger<RecommendationController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRecommendationService _recommendationService;
        private readonly IFilmSimilarityUpdateService _filmSimilarityUpdateService;

        public RecommendationController(
            ILogger<RecommendationController> logger, IUnitOfWork unitOfWork, 
            IRecommendationService recoService, IFilmSimilarityUpdateService filmSimilarityUpdateService
            )
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _recommendationService = recoService;
            _filmSimilarityUpdateService = filmSimilarityUpdateService;
        }

        public async Task<IActionResult> Recommend()
        {
            await _filmSimilarityUpdateService.UpdateSimilaritiesForFilmAsync(1);
            return View();
        }
    }
}
