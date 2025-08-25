using HotelManagement.Models;
using HotelManagement.Services;

// Seed mínimo
var rooms = new List<Room>
{
    new() { Id = 1, Number = "101", Capacity = 2, Price = 150_000m },
    new() { Id = 2, Number = "102", Capacity = 3, Price = 220_000m }
};

var customers = new List<Customer>
{
    new() { Id = 1, FullName = "Juan Perez", Email = "juan@example.com" },
    new() { Id = 2, FullName = "Ana Gómez", Email = "ana@example.com" }
};

var service = new ReservationService(rooms, customers);

// Demo corto: listar, chequear disponibilidad, crear reserva, intentar solapar
Console.WriteLine("== Habitaciones ==");
rooms.ForEach(r => Console.WriteLine(r));

var in1 = new DateOnly(2025, 9, 1);
var out1 = new DateOnly(2025, 9, 4);

Console.WriteLine($"\n¿Room 101 disponible del {in1} al {out1}? " +
                  (service.IsRoomAvailable(1, in1, out1) ? "Sí" : "No"));

var resOk = service.CreateReservation(roomId: 1, customerId: 1, checkIn: in1, checkOut: out1);
Console.WriteLine(resOk is null ? "No se pudo reservar." : $"Reserva creada: {resOk}");

var in2 = new DateOnly(2025, 9, 3);
var out2 = new DateOnly(2025, 9, 5);
Console.WriteLine($"\nIntentando reserva solapada del {in2} al {out2}...");
var resFail = service.CreateReservation(roomId: 1, customerId: 2, checkIn: in2, checkOut: out2);
Console.WriteLine(resFail is null ? "Correcto: detectado solape y no se reservó." : "ERROR: no debió permitirla.");

Console.WriteLine("\n== Reservas actuales ==");
foreach (var r in service.GetAllReservations())
    Console.WriteLine(r);

Console.WriteLine("\nListo. (MVP de estructura funcionando)");
