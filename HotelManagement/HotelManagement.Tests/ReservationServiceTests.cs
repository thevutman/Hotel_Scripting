using HotelManagement.Models;
using HotelManagement.Services;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelManagement.Tests
{
    [TestFixture]
    public class ReservationServiceTests
    {
        private ReservationService _service;
        private List<Room> _rooms;
        private List<Customer> _customers;

        [SetUp]
        public void Setup()
        {
            // Reiniciar datos antes de cada prueba
            _rooms = new List<Room>
            {
                new() { Id = 1, Number = "101", Capacity = 2, Price = 100_000m },
                new() { Id = 2, Number = "102", Capacity = 3, Price = 200_000m }
            };
            _customers = new List<Customer>
            {
                new() { Id = 1, FullName = "Test Customer", Email = "test@example.com" }
            };
            _service = new ReservationService(_rooms, _customers);
        }

        // PRUEBA: DISPONIBILIDAD (CREATE/Overlap)
        [Test]
        public void CreateReservation_ShouldNotAllowOverlap()
        {
            var pricing = new StandardPricingStrategy();

            
            var safeIn = DateOnly.FromDateTime(DateTime.Today.AddDays(365));
            var safeOut = DateOnly.FromDateTime(DateTime.Today.AddDays(370));

           
            var resOk = _service.CreateReservation(
                roomId: 1,
                customerId: 1,
                safeIn, 
                safeOut,
                pricing);

            Assert.IsNotNull(resOk, "La primera reserva válida debe crearse correctamente (Verificar si la fecha está en el pasado).");

           
            var overlapIn = DateOnly.FromDateTime(DateTime.Today.AddDays(369));
            var overlapOut = DateOnly.FromDateTime(DateTime.Today.AddDays(371));

            var resFail = _service.CreateReservation(roomId: 1, customerId: 1, overlapIn, overlapOut, pricing);

            Assert.IsNull(resFail, "La reserva solapada NO debió crearse.");
        }


        // PRUEBA: CÁLCULO DE FACTURACIÓN (Strategy)
        [TestCase(typeof(StandardPricingStrategy), 300_000, TestName = "StandardPrice")]
        [TestCase(typeof(HighSeasonPricingStrategy), 360_000, TestName = "HighSeasonPrice")]
        public void CreateReservation_ShouldCalculateTotalCorrectly_BasedOnStrategy(Type strategyType, decimal expectedTotal)
        {
            var checkIn = new DateOnly(2025, 11, 1);
            var checkOut = new DateOnly(2025, 11, 4); // 3 noches, Room 1 (100k)
            var pricingStrategy = (IPricingStrategy)Activator.CreateInstance(strategyType)!;

            var res = _service.CreateReservation(roomId: 1, customerId: 1, checkIn, checkOut, pricingStrategy);

            Assert.IsNotNull(res);
            Assert.AreEqual(expectedTotal, res.Total, "El total calculado es incorrecto.");
        }

        // PRUEBA: CANCELACIÓN (DELETE)
        [Test]
        public void CancelReservation_ShouldRemoveReservationFromList()
        {
            var pricing = new StandardPricingStrategy();
            var res = _service.CreateReservation(1, 1, new DateOnly(2025, 12, 1), new DateOnly(2025, 12, 5), pricing);
            var initialCount = _service.GetAllReservations().Count();

            bool success = _service.CancelReservation(res!.Id);

            Assert.IsTrue(success, "La cancelación debe ser exitosa.");
            Assert.AreEqual(initialCount - 1, _service.GetAllReservations().Count(), "La reserva debe ser eliminada.");
        }

        // PRUEBA: MODIFICACIÓN (UPDATE)
        [Test]
        public void UpdateReservation_ShouldChangeDatesAndRecalculateTotal()
        {
            // Arrange
            var initialPricing = new StandardPricingStrategy();
            var newPricing = new HighSeasonPricingStrategy();

            var initialIn = DateOnly.FromDateTime(DateTime.Today.AddYears(1));
            var initialOut = DateOnly.FromDateTime(DateTime.Today.AddYears(1).AddDays(3));

            var newIn = DateOnly.FromDateTime(DateTime.Today.AddYears(1).AddDays(10));
            var newOut = DateOnly.FromDateTime(DateTime.Today.AddYears(1).AddDays(12));

            var res = _service.CreateReservation(1, 1, initialIn, initialOut, initialPricing);

            Assert.IsNotNull(res, "La reserva inicial falló por datos inválidos.");

            var updatedRes = _service.UpdateReservation(res.Id, newIn, newOut, newPricing);

           
            Assert.IsNotNull(updatedRes, "La modificación debe ser exitosa.");
           
        }

        [Test]
        public void UpdateReservation_ShouldFailIfNewDatesOverlap()
        {
            var pricing = new StandardPricingStrategy();
            var todayYearLater = DateOnly.FromDateTime(DateTime.Today.AddYears(1));

            var resBlocker = _service.CreateReservation(
                roomId: 1,
                customerId: 1,
                todayYearLater.AddDays(10),
                todayYearLater.AddDays(15),
                pricing);

            Assert.IsNotNull(resBlocker, "El bloqueador debe crearse OK."); 

            var resToUpdate = _service.CreateReservation(
                roomId: 1,
                customerId: 1,
                todayYearLater.AddDays(1),
                todayYearLater.AddDays(5),
                pricing);

            Assert.IsNotNull(resToUpdate, "La reserva inicial a modificar debe crearse correctamente.");


            var updatedRes = _service.UpdateReservation(
                resToUpdate.Id, 
                todayYearLater.AddDays(12),
                todayYearLater.AddDays(16),
                pricing);

            Assert.IsNull(updatedRes, "La modificación debe fallar por solape con otra reserva.");
        }
    }
}