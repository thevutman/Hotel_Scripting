using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotelManagement.Models;

namespace HotelManagement.Services
{
    public class ReservationService
    {
        private readonly List<Room> _rooms;
        private readonly List<Customer> _customers;
        private readonly List<Reservation> _reservations = new();
        private int _nextReservationId = 1;

        public ReservationService(List<Room> rooms, List<Customer> customers)
        {
            _rooms = rooms;
            _customers = customers;
        }

        public bool IsRoomAvailable(int roomId, DateOnly from, DateOnly to)
        {
            // Rango medio abierto [from, to)
            return !_reservations.Any(r => r.RoomId == roomId && Overlap(r.CheckIn, r.CheckOut, from, to));
        }

        public Reservation? CreateReservation(int roomId, int customerId, DateOnly checkIn, DateOnly checkOut)
        {
            if (checkOut <= checkIn) return null;
            var room = _rooms.FirstOrDefault(r => r.Id == roomId);
            var customer = _customers.FirstOrDefault(c => c.Id == customerId);
            if (room is null || customer is null) return null;
            if (!IsRoomAvailable(roomId, checkIn, checkOut)) return null;

            var nights = checkOut.DayNumber - checkIn.DayNumber;
            var total = nights * room.Price;

            var res = new Reservation
            {
                Id = _nextReservationId++,
                RoomId = roomId,
                CustomerId = customerId,
                CheckIn = checkIn,
                CheckOut = checkOut,
                Total = total
            };
            _reservations.Add(res);
            return res;
        }

        public IEnumerable<Reservation> GetAllReservations() => _reservations;

        private static bool Overlap(DateOnly aStart, DateOnly aEnd, DateOnly bStart, DateOnly bEnd)
            => aStart < bEnd && bStart < aEnd; // medio abierto
    }
}
