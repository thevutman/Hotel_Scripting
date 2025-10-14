using HotelManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Services
{
    public class TaskManagementService
    {
        private readonly Queue<WorkTask> _pendingTasks = new();
        private int _nextTaskId = 1;

        public void AddTask(string description, string targetArea, EmployeeRole requiredRole)
        {
            var task = new WorkTask
            {
                Id = _nextTaskId++,
                Description = description,
                TargetArea = targetArea,
                RequiredRole = requiredRole
            };
            _pendingTasks.Enqueue(task);
        }

        public WorkTask? GetNextTask(EmployeeRole role)
        {
            return _pendingTasks.FirstOrDefault(t => t.RequiredRole == role && !t.IsCompleted);
        }

        public WorkTask? CompleteNextTask(EmployeeRole role)
        {
            var taskToComplete = _pendingTasks.FirstOrDefault(t => t.RequiredRole == role);

            if (taskToComplete == null) return null;

            // Reconstruir la cola excluyendo la tarea completada (ineficiente, pero simula el comportamiento)
            var tempTasks = new Queue<WorkTask>(_pendingTasks.Where(t => t.Id != taskToComplete.Id));
            _pendingTasks.Clear();
            foreach (var t in tempTasks) _pendingTasks.Enqueue(t);

            // Marcar y retornar la tarea (simulando que se completó)
            taskToComplete.IsCompleted = true;
            return taskToComplete;
        }

        public IEnumerable<WorkTask> GetPendingTasks() => _pendingTasks.Where(t => !t.IsCompleted);
    }
}
