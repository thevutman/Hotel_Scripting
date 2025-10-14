using HotelManagement.Models;
using HotelManagement.Services;
using System;

//===================================================================
// SETUP INICIAL
//===================================================================

// Rooms
var rooms = new List<Room>
{
    new() { Id = 1, Number = "101", Capacity = 2, Price = 150_000m },
    new() { Id = 2, Number = "102", Capacity = 3, Price = 220_000m }
};

// Customers
var customers = new List<Customer>
{
    new() { Id = 1, FullName = "Juan Perez", Email = "juan@example.com", PersonalID = "123456" },
    new() { Id = 2, FullName = "Ana Gómez", Email = "ana@example.com", PersonalID = "654321" }
};

// Employees (NUEVO)
var employees = new List<Employee>
{
    new() { Id = 10, FullName = "Marta Recepción", Role = EmployeeRole.Receptionist, Area = "Front Desk" },
    new() { Id = 20, FullName = "Carlos Limpieza", Role = EmployeeRole.Housekeeping, Area = "Piso 1" },
    new() { Id = 30, FullName = "Gerente General", Role = EmployeeRole.Manager, Area = "Todas" }
};


// Inicialización de Servicios
var reservationService = new ReservationService(rooms, customers);
var taskService = new TaskManagementService(); // NUEVO


//===================================================================
// DEMO 1: GESTIÓN DE RESERVAS CON STRATEGY
//===================================================================
Console.WriteLine("=============================================");
Console.WriteLine("== 1. DEMO: RESERVAS CON PATRÓN STRATEGY ==");
Console.WriteLine("=============================================");

var inDate = new DateOnly(2025, 11, 1);
var outDate = new DateOnly(2025, 11, 4);

// 1. Estrategia Estándar (3 noches * 150,000 = 450,000)
var standardStrategy = new StandardPricingStrategy();
var resStandard = reservationService.CreateReservation(1, 1, inDate, outDate, standardStrategy);
Console.WriteLine($"\n[Standard] Reserva 101 ({inDate}→{outDate}): {resStandard}");


// 2. Estrategia Temporada Alta (3 noches * 220,000 * 1.20 = 792,000)
var highSeasonStrategy = new HighSeasonPricingStrategy();
var resHigh = reservationService.CreateReservation(2, 2, inDate, outDate, highSeasonStrategy);
Console.WriteLine($"[High Season] Reserva 102 ({inDate}→{outDate}): {resHigh}");

// 3. Intento de solape (Falla esperada)
var solapeIn = new DateOnly(2025, 11, 3);
var solapeOut = new DateOnly(2025, 11, 5);
var resFail = reservationService.CreateReservation(1, 2, solapeIn, solapeOut, standardStrategy);
Console.WriteLine($"\n[Solape] Intentando solape en 101: {(resFail is null ? "Correcto: No se reservó." : "ERROR")}");


//===================================================================
// DEMO 2: GESTIÓN DE TAREAS CON COLAS
//===================================================================
Console.WriteLine("\n=============================================");
Console.WriteLine("== 2. DEMO: GESTIÓN DE TAREAS (COLAS) ==");
Console.WriteLine("=============================================");

// Crear Tareas
taskService.AddTask("Limpieza profunda de salida", "Room 101", EmployeeRole.Housekeeping);
taskService.AddTask("Chequear iluminación", "Lobby", EmployeeRole.Manager);
taskService.AddTask("Atender llamada de emergencia", "Front Desk", EmployeeRole.Receptionist);

Console.WriteLine($"Tareas pendientes iniciales ({taskService.GetPendingTasks().Count()}):");
taskService.GetPendingTasks().ToList().ForEach(t => Console.WriteLine($"- {t}"));

// 1. El empleado de Limpieza toma su tarea
var housekeepingTask = taskService.GetNextTask(EmployeeRole.Housekeeping);
Console.WriteLine($"\nCarlos Limpieza toma tarea: {housekeepingTask?.Description}");
if (housekeepingTask != null)
{
    var completedTask = taskService.CompleteNextTask(EmployeeRole.Housekeeping);
    Console.WriteLine($"Tarea completada: {completedTask}");
}


// 2. El Gerente toma su tarea
var managerTask = taskService.GetNextTask(EmployeeRole.Manager);
Console.WriteLine($"\nGerente toma tarea: {managerTask?.Description}");
if (managerTask != null)
{
    taskService.CompleteNextTask(EmployeeRole.Manager);
}

Console.WriteLine($"\nTareas pendientes finales ({taskService.GetPendingTasks().Count()}):");
taskService.GetPendingTasks().ToList().ForEach(t => Console.WriteLine($"- {t}"));
