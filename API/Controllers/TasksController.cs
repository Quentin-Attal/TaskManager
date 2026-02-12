using Application.Common;
using Application.DTOs;
using Application.Services.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/tasks")]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _service;

        public TasksController(ITaskService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tasks = await _service.GetAllAsync();
            var response = ApiResponse<IEnumerable<TaskItem>>
                .SuccessResponse(tasks, "Tasks retrieved successfully");
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTaskRequest request)
        {
            var task = await _service.CreateAsync(request.Title);
            var response = ApiResponse<TaskItem>
                .SuccessResponse(task, "Task created successfully");
            return Ok(response);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id)
        {
            var task = await _service.MarkDoneAsync(id);
            if (!task)
            {
                var errorResponse = ApiResponse<TaskItem>
                    .FailureResponse("Task not found");
                return NotFound(errorResponse);
            }
            var response = ApiResponse<bool>
                .SuccessResponse(task, "Task marked as done successfully");
            return Ok(response);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var task = await _service.DeleteAsync(id);
            if (!task)
            {
                var errorResponse = ApiResponse<TaskItem>
                    .FailureResponse("Task not found");
                return NotFound(errorResponse);
            }
            var response = ApiResponse<bool>
                .SuccessResponse(task, "Task deleted successfully");
            return Ok(response);
        }
    }
}
