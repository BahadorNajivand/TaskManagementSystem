using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/tasks")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ModelsAndEnums.Models.Task>>> GetTasks()
        {
            var tasks = await _taskService.GetTasks();
            return Ok(tasks);
        }

        [Authorize]
        [HttpGet("{taskId}")]
        public async Task<ActionResult<ModelsAndEnums.Models.Task>> GetTask(int taskId)
        {
            var task = await _taskService.GetTaskById(taskId);

            if (task == null)
            {
                return NotFound();
            }

            return Ok(task);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ModelsAndEnums.Models.Task>> CreateTask(ModelsAndEnums.Models.Task task)
        {
            var createdTask = await _taskService.CreateTask(task);
            return CreatedAtAction(nameof(GetTask), new { taskId = createdTask.TaskId }, createdTask);
        }

        [Authorize]
        [HttpPut("{taskId}")]
        public async Task<IActionResult> UpdateTask(int taskId, ModelsAndEnums.Models.Task task)
        {
            var updatedTask = await _taskService.UpdateTask(taskId, task);

            if (updatedTask == null)
            {
                return NotFound();
            }

            return NoContent();
        }

        [Authorize]
        [HttpDelete("{taskId}")]
        public async Task<IActionResult> DeleteTask(int taskId)
        {
            var deleted = await _taskService.DeleteTask(taskId);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
