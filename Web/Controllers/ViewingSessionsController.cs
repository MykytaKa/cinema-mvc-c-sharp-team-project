

using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Web.Controllers;

public class ViewingSessionsController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public ViewingSessionsController(IUnitOfWork unitOfWork )
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> ViewingSessions()
    {
        var repository = _unitOfWork.Repository<Film>();
        
        var films = await repository.GetAsync(includeProperties:"Genres");
        
        return View(films);
    }
    
}