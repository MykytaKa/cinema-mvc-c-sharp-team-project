using Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class FilmViewModel
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; }

        [Required]
        [Url(ErrorMessage = "Please enter a valid URL for the poster.")]
        public string PosterURL { get; set; }

        [Url(ErrorMessage = "Please enter a valid URL for the trailer.")]
        public string TrailerURL { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; }

        [Range(0, 10, ErrorMessage = "Rating must be between 0 and 10.")]
        public decimal Rating { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime ReleaseRate { get; set; }

        [Required]
        [Display(Name = "Duration (HH:MM)")]
        public string Duration { get; set; }

        [Required]
        [StringLength(5, ErrorMessage = "Age rating cannot exceed 5 characters.")]
        public string AgeRating { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Director name cannot exceed 50 characters.")]
        public string Director { get; set; }

        public List<int> SelectedActorIds { get; set; } = new List<int>();
        public List<int> SelectedGenreIds { get; set; } = new List<int>();

        public List<Actor> AllActors { get; set; } = new List<Actor>();
        public List<Genre> AllGenres { get; set; } = new List<Genre>();
        public List<Film> AllFilms { get; set; } = new List<Film>();

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }   
        public string Actors { get; set; }
        public string Genres { get; set; }
        public string SearchQuery {  get; set; }

    }
}
