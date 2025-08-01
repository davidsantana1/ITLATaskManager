﻿using ITLATaskManager.DataAccess.Data;
using ITLATaskManager.Models;
using ITLATaskManager.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ITLATaskManagerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToDoTaskController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ToDoTaskController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetTasks()
        {
            var tasks = await _context.ToDoTasks.ToListAsync();
            return Ok(tasks);
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingTasks()
        {
            var pendingTasks = await _context.ToDoTasks.Where(t => t.Status == "Pending").ToListAsync();
            return Ok(pendingTasks);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(int id)
        {
            var task = await FindTaskByIdAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            return Ok(task);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] ToDoTask<string> task)
        {
            if (task == null)
            {
                return BadRequest("Task cannot be null");
            }
            var validationResult = ValidateTaskModel(task);
            if (validationResult != null) return validationResult;

            await _context.ToDoTasks.AddAsync(task);
            await _context.SaveChangesAsync();
            Action<ToDoTask<string>> notifyCreation = task => Console.WriteLine($"Tarea creada: {task.Description}, vencimiento: {task.DueDate}");
            return CreatedAtAction(nameof(GetTaskById), new { id = task.Id }, task);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, ToDoTask<string> task)
        {
            if (id != task.Id)
            {
                return BadRequest("Task ID mismatch");
            }
            var validationResult = ValidateTaskModel(task);
            if (validationResult != null) return validationResult;

            var existingTask = await FindTaskByIdAsync(id);
            if (existingTask == null)
            {
                return NotFound();
            }
            existingTask.Title = task.Title;
            existingTask.Description = task.Description;
            existingTask.DueDate = task.DueDate;
            existingTask.Status = task.Status;
            existingTask.AdditionalData = task.AdditionalData;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await FindTaskByIdAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            _context.ToDoTasks.Remove(task);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private async Task<ToDoTask<string>?> FindTaskByIdAsync(int id)
        {
            return await _context.ToDoTasks.FindAsync(id);
        }

        private IActionResult ValidateTaskModel(ToDoTask<string> task)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (!TaskValidation.DefaultValidator(task))
                return BadRequest("Invalid task data");
            return null!;
        }
    }
}
