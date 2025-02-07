using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IFilmSimilarityUpdateService
    {
        Task UpdateSimilaritiesForFilmAsync(int filmId);
        Task DeleteFilmWithSimilaritiesAsync(int filmId);
    }
}
