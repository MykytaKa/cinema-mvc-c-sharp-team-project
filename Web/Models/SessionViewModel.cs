using Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class SessionViewModel
    {
        public int FilmId { get; set; }
        public int HallId { get; set; }
        public DateTime DateTimeBeg { get; set; }
        public DateTime DateTimeEnd { get; set; }
        public int NumberOfDays { get; set; }
        public decimal Price { get; set; }

        public List<Film> AllFilms { get; set; } = new();
        public List<Hall> AllHalls { get; set; } = new();
        public List<Session> AllSessions { get; set; } = new(); 

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string SearchQuery { get; set; }
    }
}