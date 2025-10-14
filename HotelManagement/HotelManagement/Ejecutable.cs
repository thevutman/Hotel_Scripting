using HotelManagement.Models;
using HotelManagement.Services;
using System;
using System.Linq;


// EJECUTABLE DE DEMOSTRACION COMPLETA


Console.WriteLine("  INICIO DEL SISTEMA HOTEL MANAGEMENT ");


// SETUP INICIAL DE DATOS
var rooms = new List<Room>
{
    new() { Id = 1, Number = "101", Capacity = 2, Price = 100_000m },
    new() { Id = 2, Number = "205", Capacity = 3, Price = 150_000m }
};
var customers = new List<Customer>
{
    new() { Id = 1, FullName = "Juan Perez", PersonalID = "123", Email = "juan@example.com" }
};



//demo para empleados

Console.WriteLine("\n--- 1. EMPLEADOS: DEMO FACTORY & SINGLETON (Patrones Creacionales) ---");

var initialEmployees = new List<Employee>
{
    EmployeeFactory.CreateEmployee("Marta R.", EmployeeRole.Receptionist, "marta@hotel.com", "Front Desk"),
    EmployeeFactory.CreateEmployee("Carlos L.", EmployeeRole.Housekeeping, "carlos@hotel.com", "Piso 1"),
};
var employeeService = EmployeeServiceSingleton.GetInstance(initialEmployees); 
var martaId = initialEmployees.First(e => e.Role == EmployeeRole.Receptionist).Id;

Console.WriteLine("[FACTORY] Empleados Iniciales:");
employeeService.GetAllEmployees().ToList().ForEach(e => Console.WriteLine($"- {e}"));


// Prueba de Singleton 

employeeService.AddNewEmployee("Gerente J", EmployeeRole.Manager, "ceo@hotel.com", "Todas");
var empCount = employeeService.GetAllEmployees().Count();
Console.WriteLine($"\n[SINGLETON] Nuevo empleado añadido. Total de empleados: {empCount}");


// otros servicios
var reservationService = new ReservationService(rooms, customers);
var taskService = new TaskManagementService(); 


// esta es para las reservads
Console.WriteLine("\n\n--- 2. RESERVAS: DEMO CRUD & STRATEGY (Patrón de Comportamiento) ---");

var inDate = DateOnly.FromDateTime(DateTime.Today.AddDays(10));
var outDate = DateOnly.FromDateTime(DateTime.Today.AddDays(13)); // 3 noches

// CREATE (una etrategia paraestandas)
var standardStrategy = new StandardPricingStrategy();
var res1 = reservationService.CreateReservation(1, 1, inDate, outDate, standardStrategy);
Console.WriteLine($"\n[CREATE: STANDARD] Reserva OK ({res1})");

// CREATE (una etrategia para temporada alta)
var highSeasonStrategy = new HighSeasonPricingStrategy();
var res2 = reservationService.CreateReservation(2, 1, inDate, outDate, highSeasonStrategy);
Console.WriteLine($"[CREATE: HIGH SEASON] Reserva OK ({res2})");

// UPDATE ()
var newOutDate = DateOnly.FromDateTime(DateTime.Today.AddDays(15)); // 5 noches
var updatedRes = reservationService.UpdateReservation(res1!.Id, res1.CheckIn, newOutDate, highSeasonStrategy); // Ahora con High Season
Console.WriteLine($"\n[UPDATE] Reserva {res1.Id} modificada (Total recalculado con High Season): {updatedRes}");

// DELETE ( para cancelacion)
var cancelSuccess = reservationService.CancelReservation(res2!.Id);
Console.WriteLine($"\n[DELETE] Cancelación de Reserva {res2.Id}: {(cancelSuccess ? "EXITOSA" : "FALLIDA")}");

// leer las reservas
Console.WriteLine("\n[READ] Reservas Actuales:");
reservationService.GetAllReservations().ToList().ForEach(r => Console.WriteLine($"- {r}"));


// 3. prueba de tareas , pero con colas
Console.WriteLine("\n\n--- 3. TAREAS: DEMO COLAS Y ROLES (Estructura de Datos) ---");

// 3.1 Añadir Tareas a la Cola
taskService.AddTask("Limpieza de salida (Room 101)", "Room 101", EmployeeRole.Housekeeping);
taskService.AddTask("Chequeo de caja diaria", "Front Desk", EmployeeRole.Receptionist);
taskService.AddTask("Revisar inventario de bar", "Bar", EmployeeRole.Manager);

Console.WriteLine($"Tareas en la cola ({taskService.GetPendingTasks().Count()}):");
taskService.GetPendingTasks().ToList().ForEach(t => Console.WriteLine($"- {t}"));

// 3.2 Procesamiento de Tareas por Rol
Console.WriteLine("\n--- Procesando Tareas ---");

var receptionistTask = taskService.GetNextTask(EmployeeRole.Receptionist);
if (receptionistTask != null)
{
    Console.WriteLine($"Marta R. toma y completa: {receptionistTask.Description}");
    taskService.CompleteNextTask(EmployeeRole.Receptionist);
}

Console.WriteLine($"\nTareas pendientes finales ({taskService.GetPendingTasks().Count()}):");
taskService.GetPendingTasks().ToList().ForEach(t => Console.WriteLine($"- {t}"));


Console.WriteLine("  EJECUTABLE DE AVANCE COMPLETADO.");
