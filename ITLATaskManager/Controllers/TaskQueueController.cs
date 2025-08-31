using ITLATaskManagerAPI.Security;
using ITLATaskManagerAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace ITLATaskManagerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [RequireRole]
    public class TaskQueueController : ControllerBase
    {
        private readonly ITaskQueueService _taskQueueService;

        public TaskQueueController(ITaskQueueService taskQueueService)
        {
            _taskQueueService = taskQueueService;
        }

        [HttpPost("start")]
        public IActionResult StartProcessing()
        {
            _taskQueueService.StartProcessing();
            return Ok(new { Message = "Queue processing started" });
        }

        [HttpPost("stop")]
        public IActionResult StopProcessing()
        {
            _taskQueueService.StopProcessing();
            return Ok(new { Message = "Queue processing stopped" });
        }

        [HttpGet("status")]
        public IActionResult GetQueueStatus()
        {
            return Ok(
                new
                {
                    QueueLength = _taskQueueService.QueueLength,
                    IsProcessing = _taskQueueService.IsProcessing,
                }
            );
        }
    }
}
