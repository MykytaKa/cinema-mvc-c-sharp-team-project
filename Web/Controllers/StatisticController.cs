using Application.Interfaces;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using ClosedXML.Excel;
using System.IO;
using System.Threading.Tasks;
using Web.Models;

namespace Web.Controllers
{
    public class StatisticController : Controller
    {
        private readonly ILogger<StatisticController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private const int PageSize = 10; 

        public StatisticController(ILogger<StatisticController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> FilmTicket(int page = 1, string sortBy = "name")
        {
            var films = await _unitOfWork.Repository<Film>().GetAllAsync();
            var bookings = await _unitOfWork.Repository<Booking>()
                .GetAllAsync(b => b.Include(x => x.Session));
            var tickets = await _unitOfWork.Repository<Ticket>().GetAllAsync();

            var statistics = films.Select(film => new FilmStatisticViewModel
            {
                FilmName = film.Name,
                TicketsSold = tickets.Count(ticket =>
                    bookings.Any(booking =>
                        booking.Session != null &&
                        booking.Id == ticket.BookingId &&
                        booking.Session.FilmId == film.Id))
            });

            statistics = sortBy switch
            {
                "name" => statistics.OrderBy(s => s.FilmName),
                "minTickets" => statistics.OrderBy(s => s.TicketsSold),
                "maxTickets" => statistics.OrderByDescending(s => s.TicketsSold),
                _ => statistics.OrderBy(s => s.FilmName) 
            };

            var totalItems = statistics.Count();
            var paginatedStatistics = statistics
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            var viewModel = new FilmStatisticListViewModel
            {
                Statistics = paginatedStatistics,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize),
                SortBy = sortBy
            };

            return View(viewModel);
        }

        public async Task<IActionResult> FilmOccupancy(int page = 1, string sortBy = "name", DateTime? startDate = null, DateTime? endDate = null)
        {
            var films = await _unitOfWork.Repository<Film>().GetAllAsync();
            var bookings = await _unitOfWork.Repository<Booking>()
                .GetAllAsync(b => b.Include(x => x.Session).ThenInclude(s => s.Hall));
            var tickets = await _unitOfWork.Repository<Ticket>().GetAllAsync();
            var halls = await _unitOfWork.Repository<Hall>().GetAllAsync();

            // Фільтрація за датами
            if (startDate.HasValue)
            {
                bookings = bookings.Where(b => b.Session.DateTimeBeg >= startDate.Value).ToList();
            }

            if (endDate.HasValue)
            {
                bookings = bookings.Where(b => b.Session.DateTimeBeg <= endDate.Value).ToList();
            }

            var statistics = films.Select(film =>
            {
                var filmSessions = bookings.Where(b => b.Session != null && b.Session.FilmId == film.Id)
                                            .Select(b => b.Session);

                var totalSeats = filmSessions.Sum(session =>
                    session.HallId != 0 ? halls.First(h => h.Id == session.HallId).NumberOfSeats : 0);

                var totalTicketsSold = tickets.Count(ticket =>
                    bookings.Any(booking =>
                        booking.Id == ticket.BookingId &&
                        booking.Session != null &&
                        booking.Session.FilmId == film.Id));

                var averageOccupancy = totalSeats > 0
                    ? (double)totalTicketsSold / totalSeats * 100
                    : 0;

                return new FilmOccupancyViewModel
                {
                    FilmName = film.Name,
                    AverageOccupancy = Math.Round(averageOccupancy, 2)
                };
            });

            statistics = sortBy switch
            {
                "name" => statistics.OrderBy(s => s.FilmName),
                "minOccupancy" => statistics.OrderBy(s => s.AverageOccupancy),
                "maxOccupancy" => statistics.OrderByDescending(s => s.AverageOccupancy),
                _ => statistics.OrderBy(s => s.FilmName)
            };

            var totalItems = statistics.Count();
            var paginatedStatistics = statistics
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            var viewModel = new FilmOccupancyListViewModel
            {
                Statistics = paginatedStatistics,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize),
                SortBy = sortBy,
                DateTimeBeg = startDate,
                DateTimeEnd = endDate
            };

            return View(viewModel);
        }


        public async Task<IActionResult> ExportToExcel(string sortBy = "name")
        {
            var films = await _unitOfWork.Repository<Film>().GetAllAsync();
            var bookings = await _unitOfWork.Repository<Booking>()
                .GetAllAsync(b => b.Include(x => x.Session));
            var tickets = await _unitOfWork.Repository<Ticket>().GetAllAsync();

            var statistics = films.Select(film => new FilmStatisticViewModel
            {
                FilmName = film.Name,
                TicketsSold = tickets.Count(ticket =>
                    bookings.Any(booking =>
                        booking.Session != null &&
                        booking.Id == ticket.BookingId &&
                        booking.Session.FilmId == film.Id))
            });

            statistics = sortBy switch
            {
                "name" => statistics.OrderBy(s => s.FilmName),
                "minTickets" => statistics.OrderBy(s => s.TicketsSold),
                "maxTickets" => statistics.OrderByDescending(s => s.TicketsSold),
                _ => statistics.OrderBy(s => s.FilmName)
            };

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Ticket Sales Statistics");
                worksheet.Cell(1, 1).Value = "Film";
                worksheet.Cell(1, 2).Value = "Tickets Sold";

                int row = 2;
                foreach (var film in statistics)
                {
                    worksheet.Cell(row, 1).Value = film.FilmName;
                    worksheet.Cell(row, 2).Value = film.TicketsSold;
                    row++;
                }
                string filedate = DateTime.Now.ToString("__HH.mm dd.MM.yyyy");
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Seek(0, SeekOrigin.Begin);

                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"TicketSalesStatistics_{filedate}.xlsx");
                }
            }

        }

        public async Task<IActionResult> ExportOccupancyToExcel(string sortBy = "name", DateTime? startDate = null, DateTime? endDate = null)
        {
            var films = await _unitOfWork.Repository<Film>().GetAllAsync();
            var bookings = await _unitOfWork.Repository<Booking>()
                .GetAllAsync(b => b.Include(x => x.Session).ThenInclude(s => s.Hall));
            var tickets = await _unitOfWork.Repository<Ticket>().GetAllAsync();
            var halls = await _unitOfWork.Repository<Hall>().GetAllAsync();

            if (startDate.HasValue)
            {
                bookings = bookings.Where(b => b.Session.DateTimeBeg >= startDate.Value).ToList();
            }

            if (endDate.HasValue)
            {
                bookings = bookings.Where(b => b.Session.DateTimeBeg <= endDate.Value).ToList();
            }

            var statistics = films.Select(film =>
            {
                var filmSessions = bookings.Where(b => b.Session != null && b.Session.FilmId == film.Id)
                                            .Select(b => b.Session);

                var totalSeats = filmSessions.Sum(session =>
                    session.HallId != 0 ? halls.First(h => h.Id == session.HallId).NumberOfSeats : 0);

                var totalTicketsSold = tickets.Count(ticket =>
                    bookings.Any(booking =>
                        booking.Id == ticket.BookingId &&
                        booking.Session != null &&
                        booking.Session.FilmId == film.Id));

                var averageOccupancy = totalSeats > 0
                    ? (double)totalTicketsSold / totalSeats * 100
                    : 0;

                return new FilmOccupancyViewModel
                {
                    FilmName = film.Name,
                    AverageOccupancy = Math.Round(averageOccupancy, 2)
                };
            });

            statistics = sortBy switch
            {
                "name" => statistics.OrderBy(s => s.FilmName),
                "minOccupancy" => statistics.OrderBy(s => s.AverageOccupancy),
                "maxOccupancy" => statistics.OrderByDescending(s => s.AverageOccupancy),
                _ => statistics.OrderBy(s => s.FilmName)
            };

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Occupancy Statistics");
                worksheet.Cell(1, 1).Value = "Film";
                worksheet.Cell(1, 2).Value = "Average Occupancy (%)";

                int row = 2;
                foreach (var film in statistics)
                {
                    worksheet.Cell(row, 1).Value = film.FilmName;
                    worksheet.Cell(row, 2).Value = $"{film.AverageOccupancy}%";
                    row++;
                }

                string filedate = DateTime.Now.ToString("__HH.mm dd.MM.yyyy");
                string filterInfo = "";

                if (startDate.HasValue || endDate.HasValue)
                {
                    filterInfo = $"_from-{startDate?.ToString("dd-MM-yyyy") ?? "start"}-to-{endDate?.ToString("dd-MM-yyyy") ?? "end"}";
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Seek(0, SeekOrigin.Begin);

                    return File(stream.ToArray(),
                                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                $"OccupancyStatistics{filterInfo}{filedate}.xlsx");
                }

            }
        }

        public async Task<IActionResult> SessionPopularity(DateTime? startDate = null, DateTime? endDate = null)
        {
            var bookings = await _unitOfWork.Repository<Booking>()
                .GetAllAsync(b => b.Include(x => x.Session));

            if (startDate.HasValue)
            {
                bookings = bookings.Where(b => b.Session.DateTimeBeg >= startDate.Value).ToList();
            }

            if (endDate.HasValue)
            {
                bookings = bookings.Where(b => b.Session.DateTimeBeg <= endDate.Value).ToList();
            }

            var sessionTimes = bookings.GroupBy(b =>
            {
                var hour = b.Session.DateTimeBeg.Hour;
                if (hour >= 6 && hour < 12) return "Morning (6 AM - 12 PM)";
                if (hour >= 12 && hour < 18) return "Afternoon (12 PM - 6 PM)";
                if (hour >= 18 && hour < 24) return "Evening (6 PM - 12 AM)";
                return "Night (12 AM - 6 AM)";
            })
            .Select(g => new SessionPopularityViewModel
            {
                TimePeriod = g.Key,
                TotalBookings = g.Count()
            })
            .OrderByDescending(s => s.TotalBookings)
            .ToList();

            return View(new SessionPopularityListViewModel
            {
                Statistics = sessionTimes,
                DateTimeBeg = startDate,
                DateTimeEnd = endDate
            });
        }



    }
}
