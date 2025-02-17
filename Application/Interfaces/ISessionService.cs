using Core.Entities;
using Core.FiltersModels;


namespace Application.Interfaces;

public interface ISessionService
{
    Task<IEnumerable<Session>> GetFilteredSessionsAsync(SessionFilterModel filter);
    Task<IEnumerable<string>> GetAllGenresAsync();
    Task<IEnumerable<string>> GetAllAgeRatingsAsync();
    Task<SessionFilterModel> GetFilteredFilmsAsync(SessionFilterModel filter);
}