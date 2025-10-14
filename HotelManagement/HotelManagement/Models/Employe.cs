using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Models
{
    public enum EmployeeRole { Receptionist, Housekeeping, Manager, Admin }

    public class Employee
    {
        public int Id { get; init; }
        public string FullName { get; init; } = "";
        public string Email { get; init; } = "";
        public EmployeeRole Role { get; init; }
        public string Area { get; init; } = "General";

        public override string ToString() => $"{FullName} ({Role}) - Area: {Area}";
    }
}
