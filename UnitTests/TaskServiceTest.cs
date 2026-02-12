using Moq;
using Application.Services;
using Application.Interfaces;
using Domain.Entities;

namespace UnitTests
{
    public class TaskServiceTest
    {
        [Fact]
        public async Task Should_GetAllAsync()
        {
            var repoMock = new Mock<ITaskRepository>();
            var handler = new TaskService(repoMock.Object);

            var result = await handler.GetAllAsync();

            Assert.NotNull(result);
            Assert.IsType<IEnumerable<TaskItem>>(result, exactMatch: false);
            repoMock.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task Should_GetByIdAsync()
        {
            var repoMock = new Mock<ITaskRepository>();
            var handler = new TaskService(repoMock.Object);
            var id = Guid.NewGuid();
            var expectedTask = new TaskItem { Id = id, Title = "Test", IsDone = false, CreatedAtUtc = DateTime.UtcNow };
            repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(expectedTask);

            var result = await handler.GetByIdAsync(id);

            Assert.NotNull(result);
            Assert.IsType<TaskItem?>(result);
            repoMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Once);

            id = Guid.NewGuid();
            result = await handler.GetByIdAsync(id);

            Assert.Null(result);
            repoMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Exactly(2));
        }

        [Fact]
        public async Task Should_GetAddAsync()
        {
            var repoMock = new Mock<ITaskRepository>();
            var handler = new TaskService(repoMock.Object);

            var result = await handler.CreateAsync("hello");

            Assert.NotNull(result);
            Assert.IsType<TaskItem>(result);
            repoMock.Verify(r => r.AddAsync(It.IsAny<TaskItem>()), Times.Once);
        }

        [Fact]
        public async Task Should_MaskAsDoneAsync()
        {
            var repoMock = new Mock<ITaskRepository>();
            var handler = new TaskService(repoMock.Object);
            var id = Guid.NewGuid();
            var expectedTask = new TaskItem { Id = id, Title = "Test", IsDone = true, CreatedAtUtc = DateTime.UtcNow };
            repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(expectedTask);

            var result = await handler.MarkDoneAsync(id);

            Assert.True(result);
            Assert.IsType<Boolean>(result);
            repoMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
            repoMock.Verify(r => r.UpdateAsync(It.IsAny<TaskItem>()), Times.Once);

            id = Guid.NewGuid();
            result = await handler.MarkDoneAsync(id);

            Assert.False(result);
            repoMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Exactly(2));
            repoMock.Verify(r => r.UpdateAsync(It.IsAny<TaskItem>()), Times.Once);
        }

        [Fact]
        public async Task Should_DeleteAsync()
        {
            var repoMock = new Mock<ITaskRepository>();
            var handler = new TaskService(repoMock.Object);
            var id = Guid.NewGuid();
            var expectedTask = new TaskItem { Id = id, Title = "Test", IsDone = true, CreatedAtUtc = DateTime.UtcNow };
            repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(expectedTask);

            var result = await handler.DeleteAsync(id);

            Assert.True(result);
            Assert.IsType<Boolean>(result);
            repoMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
            repoMock.Verify(r => r.DeleteAsync(It.IsAny<Guid>()), Times.Once);

            id = Guid.NewGuid();
            result = await handler.DeleteAsync(id);

            Assert.False(result);
            repoMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Exactly(2));
            repoMock.Verify(r => r.DeleteAsync(It.IsAny<Guid>()), Times.Once);
        }
    }
}
