using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Models
{
    public class WorkTask
    {
        public int Id { get; init; }
        public string Description { get; init; } = "";
        public string TargetArea { get; init; } = ""; // e.g., "Room 101", "Pool", "Lobby"
        public EmployeeRole RequiredRole { get; init; }
        public bool IsCompleted { get; set; } = false;

        public override string ToString() => $"Task #{Id}: {Description} en {TargetArea} ({RequiredRole}) - Completada: {IsCompleted}";
    }
}
