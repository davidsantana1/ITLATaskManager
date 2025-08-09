using ITLATaskManager.DataAccess.Data;
using ITLATaskManager.Models;
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(int id)
        {
            var task = await _context.ToDoTasks.FirstOrDefaultAsync(t => t.Id == id);
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
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _context.ToDoTasks.AddAsync(task);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTaskById), new { id = task.Id }, task);
        }

        [HttpPost("createHighPriority")]
        public async Task<IActionResult> CreateHighPriorityTask([FromBody] CreateTaskDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Description) && string.IsNullOrWhiteSpace(dto.Title))
            {
                return BadRequest("Description cannot be null or empty");
            }

            var task = ToDoTaskFactory.CreateHighPriorityTask(dto.Title, dto.Description);
            await _context.ToDoTasks.AddAsync(task);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTaskById), new { id = task.Id }, task);
        }

        [HttpPost("createLowPriority")]
        public async Task<IActionResult> CreateLowPriorityTask([FromBody] CreateTaskDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Description) && string.IsNullOrWhiteSpace(dto.Title))
            {
                return BadRequest("Description cannot be null or empty");
            }
            var task = ToDoTaskFactory.CreateLowPriorityTask(dto.Title, dto.Description);
            await _context.ToDoTasks.AddAsync(task);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTaskById), new { id = task.Id }, task);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, ToDoTask<string> task)
        {
            if (id != task.Id)
            {
                return BadRequest("Task ID mismatch");
            }
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingTask = await _context.ToDoTasks.FirstOrDefaultAsync(t => t.Id == id);
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
            var task = await _context.ToDoTasks.FirstOrDefaultAsync(t => t.Id == id);
            if (task == null)
            {
                return NotFound();
            }

            _context.ToDoTasks.Remove(task);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
