using Core.Entities;
using Core.Interfaces;
using Core.Models;
using System.Security.Claims;

namespace Infrastructure.Services
{
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
            var session = await _unitOfWork.Repository<Session>()
                .GetAsync(s => s.Id == sessionId, includeProperties: "Film,Hall.Seats,Bookings.Tickets.Seat");

            if (!session.Any()) return null;

            var sessionData = session.First();

            var bookedSeatIds = sessionData.Bookings
                .Where(b => b.StatusId != 3)  // Якщо StatusId = 3, місце НЕ вважається зайнятим
                .SelectMany(b => b.Tickets)
                .Select(t => t.SeatId)
                .ToHashSet();

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

        // Метод для підтвердження бронювання із DTO
        public async Task<Booking> ConfirmBookingAsync(CreateBookingDto bookingDto)
        {
            Console.WriteLine($"➡ Починаємо бронювання: UserId={bookingDto.UserId}, SessionId={bookingDto.SessionId}");

            if (bookingDto.UserId == 0)
            {
                Console.WriteLine("Помилка: UserId = 0!");
                return null;
            }

            var session = await _unitOfWork.Repository<Session>()
                .GetAsync(s => s.Id == bookingDto.SessionId, includeProperties: "Film,Bookings.Tickets.Seat");

            var enumerable = session as Session[] ?? session.ToArray();
            if (enumerable.Length == 0)
            {
                Console.WriteLine("Помилка: Сеанс не знайдено!");
                return null;
            }

            var sessionData = enumerable.First();

            var bookedSeatIds = sessionData.Bookings
                .Where(b => b.StatusId != 3)
                .SelectMany(b => b.Tickets)
                .Select(t => t.SeatId)
                .ToHashSet();

            var alreadyBookedSeats = bookingDto.SeatIds.Where(seatId => bookedSeatIds.Contains(seatId)).ToList();
            if (alreadyBookedSeats.Count != 0)
            {
                Console.WriteLine($"Помилка: Місця {string.Join(", ", alreadyBookedSeats)} вже зайняті!");
                return null;
            }

            var selectedSeats = await _unitOfWork.Repository<Seat>()
                .GetAsync(s => bookingDto.SeatIds.Contains(s.Id));

            var seats = selectedSeats as Seat[] ?? selectedSeats.ToArray();
            if (seats.Length == 0)
            {
                Console.WriteLine("Помилка: Жодне місце не знайдено!");
                return null;
            }

            var totalPrice = seats.Count() * sessionData.Price;

            var booking = new Booking
            {
                UserId = bookingDto.UserId,
                SessionId = sessionData.Id,
                DateTime = DateTime.Now,
                Price = totalPrice,
                StatusId = 1,
                Tickets = seats.Select(seat => new Ticket { SeatId = seat.Id }).ToList()
            };

            await _unitOfWork.Repository<Booking>().InsertAsync(booking);
            await _unitOfWork.SaveAsync();

            Console.WriteLine("Бронювання успішне!");
            return booking;
        }

        
        // Новий метод, який переносить логіку з контролера в сервіс
        public async Task<(bool IsSuccess, string ErrorMessage, Booking? Booking)> ConfirmBookingAsync(BookSessionViewModel model, ClaimsPrincipal user)
        {
            // Перевірка: чи вибрано місця
            if (model.SelectedSeats.Count == 0)
            {
                return (false, "Ви не вибрали місця!", null);
            }

            // Отримання UserId з Claims
            var userIdClaim = user.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                Console.WriteLine($"Помилка: UserId не є числом! Отримано: {userIdClaim}");
                return (false, "Помилка авторизації!", null);
            }

            Console.WriteLine($"Авторизований користувач: {userId}");

            // Формуємо DTO для бронювання
            var bookingDto = new CreateBookingDto
            {
                UserId = userId,
                SessionId = model.SessionId,
                SeatIds = model.SelectedSeats
            };

            // Викликаємо внутрішній метод підтвердження бронювання
            var booking = await ConfirmBookingAsync(bookingDto);

            return (true, string.Empty, booking);
        }
    }
}
