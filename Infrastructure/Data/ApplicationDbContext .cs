using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities;

namespace Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<User_Type> User_Types { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Hall> Halls { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Film> Films { get; set; }
        public DbSet<Actor> Actors { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<FilmSimilarity> FilmSimilarities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Конфігурація каскадного видалення для таблиці FilmSimilarity
            modelBuilder.Entity<FilmSimilarity>()
                .HasOne(fs => fs.Film1)
                .WithMany()
                .HasForeignKey(fs => fs.Film1Id)
                .OnDelete(DeleteBehavior.Restrict); // Забороняє каскадне видалення

            modelBuilder.Entity<FilmSimilarity>()
                .HasOne(fs => fs.Film2)
                .WithMany()
                .HasForeignKey(fs => fs.Film2Id)
                .OnDelete(DeleteBehavior.Restrict); // Забороняє каскадне видалення

            // Конфігурація каскадного видалення для таблиці Ticket
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Booking)
                .WithMany(b => b.Tickets)
                .HasForeignKey(t => t.BookingId)
                .OnDelete(DeleteBehavior.Cascade); // Це залишається каскадним

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Seat)
                .WithMany(s => s.Tickets)
                .HasForeignKey(t => t.SeatId)
                .OnDelete(DeleteBehavior.Restrict); // Тут вимикаємо каскадне видалення
            
            modelBuilder.Entity<Session>()
                .HasOne(s => s.Film)
                .WithMany(f => f.Sessions)
                .HasForeignKey(s => s.FilmId);

            modelBuilder.Entity<Session>()
                .HasOne(s => s.Hall)
                .WithMany(h => h.Sessions)
                .HasForeignKey(s => s.HallId);

            // Додаткові конфігурації, якщо необхідно
            base.OnModelCreating(modelBuilder);
        }
    }
}
