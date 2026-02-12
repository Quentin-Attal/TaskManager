using Application.Common;
using Contracts.Tasks;
using Domain.Entities;
using FluentAssertions;
using Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace IntegrationTests
{
    public class TasksControllerTest : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;
        private readonly Guid _taskId = Guid.NewGuid();
        private readonly Guid _userId = Guid.NewGuid();

        public TasksControllerTest(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
            _factory = factory;

            _client.DefaultRequestHeaders.Add("x-test-userid", _userId.ToString());
        }

        public async ValueTask InitializeAsync()
        {
            await _factory.ResetDatabaseAsync();

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            db.Tasks.Add(new TaskItem
            {
                Id = _taskId,
                Title = "Seed 1",
                CreatedAtUtc = DateTime.UtcNow,
                IsDone = false,
                AppUser = new AppUser
                {
                    Id = _userId,
                    Email = "mail.com",
                    PasswordHash = "hash",
                    CreatedAtUtc = DateTime.UtcNow,
                },
            });

            await db.SaveChangesAsync();
        }
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;


        [Fact]
        public async Task Get_Task_Should_Return_200()
        {
            var cancellationToken = TestContext.Current.CancellationToken;
            var response = await _client.GetAsync("/api/tasks", cancellationToken);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<TaskItem>>>(cancellationToken: cancellationToken);

            apiResponse.Should().NotBeNull();
            apiResponse.Success.Should().BeTrue();
            apiResponse.Data.Should().NotBeNull();
            apiResponse.Data.Should().HaveCountGreaterThan(0);
            apiResponse.Data.Should().Contain(t => t.Title == "Seed 1");
            apiResponse.Message.Should().Be("Tasks retrieved successfully");
        }

        [Fact]
        public async Task POST_Task_Should_Return_200()
        {
            var cancellationToken = TestContext.Current.CancellationToken;
            var createTaskRequest = new CreateTaskRequest { Title = "title" };
            var response = await _client.PostAsJsonAsync("/api/tasks", createTaskRequest, cancellationToken: cancellationToken);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<TaskItem>>(cancellationToken: cancellationToken);

            apiResponse.Should().NotBeNull();
            apiResponse.Success.Should().BeTrue();

            apiResponse.Data.Should().NotBeNull();
            apiResponse.Data!.Title.Should().Be("title");
            apiResponse.Data.IsDone.Should().BeFalse();

            apiResponse.Message.Should().Be("Task created successfully");
        }

        [Fact]
        public async Task PUT_Task_Should_Return_200()
        {
            var cancellationToken = TestContext.Current.CancellationToken;
            _ = new CreateTaskRequest { Title = "title" };
            var response = await _client.PutAsJsonAsync("/api/tasks/" + _taskId, new object(), cancellationToken: cancellationToken);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>(cancellationToken: cancellationToken);

            apiResponse.Should().NotBeNull();
            apiResponse.Success.Should().BeTrue();

            apiResponse.Data.Should().BeTrue();

            apiResponse.Message.Should().Be("Task marked as done successfully");
        }

        [Fact]
        public async Task Delete_Task_Should_Return_200()
        {
            var cancellationToken = TestContext.Current.CancellationToken;
            _ = new CreateTaskRequest { Title = "title" };
            var response = await _client.DeleteAsync("/api/tasks/" + _taskId, cancellationToken: cancellationToken);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>(cancellationToken: cancellationToken);

            apiResponse.Should().NotBeNull();
            apiResponse.Success.Should().BeTrue();

            apiResponse.Data.Should().BeTrue();

            apiResponse.Message.Should().Be("Task deleted successfully");
        }
    }
}
