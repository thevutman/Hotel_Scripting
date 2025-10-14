using HotelManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Services
{
    // Patrón Strategy: Interfaz
    public interface IPricingStrategy
    {
        decimal CalculateTotal(Room room, DateOnly checkIn, DateOnly checkOut);
    }

    internal static class PricingHelper
    {
        public static int GetNights(DateOnly checkIn, DateOnly checkOut)
        {
            return checkOut.DayNumber - checkIn.DayNumber;
        }
    }

    // Estrategia Concreta 1: Precio estándar por noche
    public class StandardPricingStrategy : IPricingStrategy
    {
        public decimal CalculateTotal(Room room, DateOnly checkIn, DateOnly checkOut)
        {
            var nights = PricingHelper.GetNights(checkIn, checkOut);
            return nights * room.Price;
        }
    }

    // Estrategia Concreta 2: Temporada Alta (20% de recargo)
    public class HighSeasonPricingStrategy : IPricingStrategy
    {
        private const decimal Surcharge = 1.20m;

        public decimal CalculateTotal(Room room, DateOnly checkIn, DateOnly checkOut)
        {
            var nights = PricingHelper.GetNights(checkIn, checkOut);
            // Aplica el recargo del 20%
            return nights * room.Price * Surcharge;
        }
    }

    // Estrategia Concreta 3: Descuento Corporativo (10% de descuento)
    public class CorporateDiscountStrategy : IPricingStrategy
    {
        private const decimal DiscountFactor = 0.90m;

        public decimal CalculateTotal(Room room, DateOnly checkIn, DateOnly checkOut)
        {
            var nights = PricingHelper.GetNights(checkIn, checkOut);
            // Aplica el descuento
            return nights * room.Price * DiscountFactor;
        }
    }
}
