using Application.DTOs;

namespace Application.Interfaces
{
    public interface IReleasesService
    {
        Task<IEnumerable<NewFilmDTO>> GetNewReleases(int month, int year);
    }
}
