using Core.Entities;

namespace Core.FiltersModels;

public class SessionFilterModel
{
    public DateTime? SessionDate { get; set; }  // Дата сеансу
    public TimeSpan? SessionTime { get; set; }  // Час сеансу
    public string Genre { get; set; }           // Жанр
    public double? MinRating { get; set; }      // Мінімальний рейтинг
    public double? MaxRating { get; set; }      // Максимальний рейтинг
    public int? MinDuration { get; set; }       // Мінімальна тривалість (хв)
    public int? MaxDuration { get; set; }       // Максимальна тривалість (хв)
    public string AgeRating { get; set; }       // Віковий рейтинг
    public DateTime? ReleaseDate { get; set; }  // Дата виходу фільму
    
    public List<string> AvailableGenres { get; set; } = new();
    public List<string> AvailableAgeRatings { get; set; } = new();
    public IEnumerable<Film> Films { get; set; } = new List<Film>();
    public IEnumerable<Session> Sessions { get; set; } = new List<Session>();
}