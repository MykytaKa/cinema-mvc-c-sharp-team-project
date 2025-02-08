using Core.Entities;
using Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IFilmInfoService
    {
        Task<Film> FilmInfoAsync(int filmId);
    }
}