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

    public ViewingSessionsController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> ViewingSessions()
    {
        var sessions = await _unitOfWork.Repository<Session>()
            .GetAsync(includeProperties: "Film.Genres,Hall,Bookings");

        var uniqueFilms = sessions
            .Select(s => s.Film)
            .Distinct()
            .ToList();

        return View(uniqueFilms);
    }
}