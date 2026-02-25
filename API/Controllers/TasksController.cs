using API.Common.Extensions;
using Application.Common;
using Application.Tasks.Interfaces;
using AutoMapper;
using Contracts.Tasks;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/tasks")]
    public class TasksController(ITaskService service, IMapper mapper) : ControllerBase
    {
        private readonly ITaskService _service = service;
        private readonly IMapper _mapper = mapper;

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var userId = User.GetUserId();
            var tasks = await _service.GetAllAsync(userId, ct);
            var answer = _mapper.Map<IEnumerable<TaskAnswers>>(tasks);
            var response = ApiResponse<IEnumerable<TaskAnswers>>
                .SuccessResponse(answer, "Tasks retrieved successfully");
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTaskRequest request, CancellationToken ct)
        {
            var userId = User.GetUserId();

            var task = await _service.CreateAsync(userId, request.Title, ct);
            var answer = _mapper.Map<TaskAnswers>(task);
            var response = ApiResponse<TaskAnswers>
                .SuccessResponse(answer, "Task created successfully");
            return CreatedAtAction(nameof(GetById), new { id = task.Id }, response);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var userId = User.GetUserId();

            var task = await _service.GetByIdAsync(userId, id, ct);
            var answer = _mapper.Map<TaskAnswers>(task);
            var response = ApiResponse<TaskAnswers>
                 .SuccessResponse(answer, "Task retrieved successfully");
            return Ok(response);
        }

        [HttpPut("{id:guid}/done")]
        public async Task<IActionResult> MarkDone(Guid id, CancellationToken ct)
        {
            var userId = User.GetUserId();

            await _service.MarkDoneAsync(userId, id, ct);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            var userId = User.GetUserId();

            await _service.DeleteAsync(userId, id, ct);
            return NoContent();
        }
    }
}
