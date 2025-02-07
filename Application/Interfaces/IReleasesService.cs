using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities;
using Core.Models;

namespace Application.Interfaces
{
    public interface IReleasesService
    {
        Task<IEnumerable<NewFilmDTO>> GetNewReleases(int month, int year);
    }
}
