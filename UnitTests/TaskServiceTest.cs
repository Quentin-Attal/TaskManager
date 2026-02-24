using Application.Repositories;
using Application.Tasks.Services;
using Domain.Entities;
using Moq;

namespace UnitTests
{
    public class TaskServiceTest
    {
        [Fact]
        public async Task Should_GetAllAsync()
        {
            var cancellationToken = TestContext.Current.CancellationToken;
            var userId = Guid.NewGuid();

            var repoMock = new Mock<ITaskRepository>();
            var handler = new TaskService(repoMock.Object);

            var result = await handler.GetAllAsync(userId, cancellationToken);

            Assert.NotNull(result);
            Assert.IsType<IEnumerable<TaskItem>>(result, exactMatch: false);
            repoMock.Verify(r => r.GetAllAsync(userId, cancellationToken), Times.Once);
        }

        [Fact]
        public async Task Should_GetByIdAsync()
        {
            var cancellationToken = TestContext.Current.CancellationToken;
            var userId = Guid.NewGuid();

            var repoMock = new Mock<ITaskRepository>();
            var handler = new TaskService(repoMock.Object);
            var id = Guid.NewGuid();
            var expectedTask = new TaskItem { Id = id, Title = "Test", IsDone = false, CreatedAtUtc = DateTime.UtcNow };
            repoMock.Setup(r => r.GetByIdAsync(userId, id, cancellationToken)).ReturnsAsync(expectedTask);

            var result = await handler.GetByIdAsync(userId, id, cancellationToken);

            Assert.NotNull(result);
            Assert.IsType<TaskItem?>(result);
            repoMock.Verify(r => r.GetByIdAsync(userId, It.IsAny<Guid>(), cancellationToken), Times.Once);

            id = Guid.NewGuid();
            result = await handler.GetByIdAsync(userId, id, cancellationToken);

            Assert.Null(result);
            repoMock.Verify(r => r.GetByIdAsync(userId, It.IsAny<Guid>(), cancellationToken), Times.Exactly(2));
        }

        [Fact]
        public async Task Should_GetAddAsync()
        {
            var cancellationToken = TestContext.Current.CancellationToken;
            var userId = Guid.NewGuid();

            var repoMock = new Mock<ITaskRepository>();
            var handler = new TaskService(repoMock.Object);

            var result = await handler.CreateAsync(userId, "hello", cancellationToken);

            Assert.NotNull(result);
            Assert.IsType<TaskItem>(result);
            repoMock.Verify(r => r.AddAsync(It.IsAny<TaskItem>()), Times.Once);
            repoMock.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Once);

        }

        [Fact]
        public async Task Should_MaskAsDoneAsync()
        {
            var cancellationToken = TestContext.Current.CancellationToken;
            var userId = Guid.NewGuid();

            var repoMock = new Mock<ITaskRepository>();
            var handler = new TaskService(repoMock.Object);
            var id = Guid.NewGuid();
            var expectedTask = new TaskItem { Id = id, Title = "Test", IsDone = true, CreatedAtUtc = DateTime.UtcNow };
            repoMock.Setup(r => r.GetByIdAsync(userId, id, cancellationToken)).ReturnsAsync(expectedTask);

            var result = await handler.MarkDoneAsync(userId, id, cancellationToken);

            Assert.IsType<Boolean>(result);
            Assert.True(result);
            repoMock.Verify(r => r.GetByIdAsync(userId, It.IsAny<Guid>(), cancellationToken), Times.Once);
            repoMock.Verify(r => r.UpdateAsync(It.IsAny<TaskItem>()), Times.Once);
            repoMock.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Once);

            id = Guid.NewGuid();
            result = await handler.MarkDoneAsync(userId, id, cancellationToken);

            Assert.False(result);
            repoMock.Verify(r => r.GetByIdAsync(userId, It.IsAny<Guid>(), cancellationToken), Times.Exactly(2));
            repoMock.Verify(r => r.UpdateAsync(It.IsAny<TaskItem>()), Times.Once);
        }

        [Fact]
        public async Task Should_DeleteAsync()
        {
            var cancellationToken = TestContext.Current.CancellationToken;
            var userId = Guid.NewGuid();

            var repoMock = new Mock<ITaskRepository>();
            var handler = new TaskService(repoMock.Object);
            var id = Guid.NewGuid();
            var expectedTask = new TaskItem { Id = id, Title = "Test", IsDone = true, CreatedAtUtc = DateTime.UtcNow };
            repoMock.Setup(r => r.GetByIdAsync(userId, id, cancellationToken)).ReturnsAsync(expectedTask);

            var result = await handler.DeleteAsync(userId, id, cancellationToken);

            Assert.IsType<Boolean>(result);
            Assert.True(result);
            repoMock.Verify(r => r.GetByIdAsync(userId, It.IsAny<Guid>(), cancellationToken), Times.Once);
            repoMock.Verify(r => r.DeleteAsync(userId, It.IsAny<Guid>(), cancellationToken), Times.Once);
            repoMock.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Once);

            id = Guid.NewGuid();
            result = await handler.DeleteAsync(userId, id, cancellationToken);

            Assert.False(result);
            repoMock.Verify(r => r.GetByIdAsync(userId, It.IsAny<Guid>(), cancellationToken), Times.Exactly(2));
            repoMock.Verify(r => r.DeleteAsync(userId, It.IsAny<Guid>(), cancellationToken), Times.Once);
        }
    }
}
