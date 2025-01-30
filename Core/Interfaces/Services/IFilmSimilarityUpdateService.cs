using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Services
{
    public interface IFilmSimilarityUpdateService
    {
        Task UpdateSimilaritiesForFilmAsync(int filmId);
        Task DeleteFilmWithSimilaritiesAsync(int filmId);
    }
}
