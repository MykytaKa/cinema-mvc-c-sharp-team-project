using Core.Entities;
using Core.Interfaces;
using Core.Models;

namespace Infrastructure.Services;

public class BookingService : IBookingService
{
    private readonly IUnitOfWork _unitOfWork;

    public BookingService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    // Метод для підготовки даних для сторінки бронювання
    public async Task<BookSessionViewModel> PrepareBookingAsync(int sessionId)
    {
        // Завантаження сеансу разом із фільмом, залом, місцями та бронюваннями
        var session = await _unitOfWork.Repository<Session>()
            .GetAsync(s => s.Id == sessionId, includeProperties: "Film,Hall.Seats,Bookings.Tickets.Seat");

        if (!session.Any()) return null; // Перевірка, чи знайдено сеанс

        var sessionData = session.First(); // Оскільки GetAsync повертає IEnumerable, беремо перший елемент

        var bookedSeatIds = sessionData.Bookings
            .Where(b => b.StatusId != 3)  // Якщо StatusId = 3, місце НЕ вважається зайнятим
            .SelectMany(b => b.Tickets)
            .Select(t => t.SeatId)
            .ToHashSet();

        // Формування моделі для відображення на сторінці
        return new BookSessionViewModel
        {
            SessionId = sessionData.Id,
            FilmName = sessionData.Film.Name,
            HallName = sessionData.Hall.Name,
            SessionDate = sessionData.DateTimeBeg,
            SessionPrice = sessionData.Price,
            AvailableSeats = sessionData.Hall.Seats
                .Select(seat => new SeatViewModel
                {
                    SeatId = seat.Id,
                    Row = seat.Row,
                    Column = seat.Column,
                    IsBooked = bookedSeatIds.Contains(seat.Id)
                })
                .ToList()
        };
    }

    // Метод для підтвердження бронювання
   public async Task<Booking> ConfirmBookingAsync(CreateBookingDto bookingDto)
{
    Console.WriteLine($"➡ Починаємо бронювання: UserId={bookingDto.UserId}, SessionId={bookingDto.SessionId}");

    if (bookingDto.UserId == 0)
    {
        Console.WriteLine("Помилка: UserId = 0!");
        return null;
    }

    var userExists = await _unitOfWork.Repository<User>().GetByIDAsync(bookingDto.UserId);
    if (userExists == null)
    {
        Console.WriteLine("Помилка: Користувач не знайдений!");
        return null;
    }

    var session = await _unitOfWork.Repository<Session>()
        .GetAsync(s => s.Id == bookingDto.SessionId, includeProperties: "Film,Bookings.Tickets.Seat");

    if (session == null || !session.Any())
    {
        Console.WriteLine("Помилка: Сеанс не знайдено!");
        return null;
    }

    var sessionData = session.First();

    //Отримуємо зайняті місця
    var bookedSeatIds = sessionData.Bookings
        .SelectMany(b => b.Tickets)
        .Select(t => t.SeatId)
        .ToHashSet();

    // Перевіряємо, чи вже зайняті місця, які обрав користувач
    var alreadyBookedSeats = bookingDto.SeatIds.Where(seatId => bookedSeatIds.Contains(seatId)).ToList();
    if (alreadyBookedSeats.Any())
    {
        Console.WriteLine($"Помилка: Місця {string.Join(", ", alreadyBookedSeats)} вже зайняті!");
        return null;
    }

    var selectedSeats = await _unitOfWork.Repository<Seat>()
        .GetAsync(s => bookingDto.SeatIds.Contains(s.Id));

    if (!selectedSeats.Any())
    {
        Console.WriteLine("Помилка: Жодне місце не знайдено!");
        return null;
    }

    var totalPrice = selectedSeats.Count() * sessionData.Price;

    var booking = new Booking
    {
        UserId = bookingDto.UserId,
        SessionId = sessionData.Id,
        DateTime = DateTime.Now,
        Price = totalPrice,
        StatusId = 1,
        Tickets = selectedSeats.Select(seat => new Ticket { SeatId = seat.Id }).ToList()
    };


    await _unitOfWork.Repository<Booking>().InsertAsync(booking);
    await _unitOfWork.SaveAsync();

    Console.WriteLine("Бронювання успішне!");
    return booking;
}



    // Метод для отримання бронювань користувача
    public async Task<List<BookingViewModel>> GetUserBookingsAsync(int userId)
    {
        var bookings = await _unitOfWork.Repository<Booking>()
            .GetAsync(
                b => b.UserId == userId,
                includeProperties: "Session.Film,Session.Hall,Tickets.Seat,Status"
            );

        return bookings.Select(b => new BookingViewModel
        {
            Id = b.Id,
            DateTime = b.DateTime,
            Price = b.Price,
            FilmName = b.Session.Film.Name,
            HallName = b.Session.Hall.Name,
            SessionStart = b.Session.DateTimeBeg,
            SessionEnd = b.Session.DateTimeEnd,
            Status = b.Status.Name,
            Seats = b.Tickets.Select(t => $"Ряд {t.Seat.Row}, Місце {t.Seat.Column}").ToList()
        }).ToList();
    }
}