using Application.Common.Exceptions;
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
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            repoMock
                .Setup(r => r.GetAllAsync(userId, cancellationToken))
                .ReturnsAsync([]);
            var handler = new TaskService(repoMock.Object, unitOfWorkMock.Object);


            var result = await handler.GetAllAsync(userId, cancellationToken);

            Assert.NotNull(result);
            Assert.IsType<List<TaskItem>>(result, exactMatch: false);
            repoMock.Verify(r => r.GetAllAsync(userId, cancellationToken), Times.Once);
        }

        [Fact]
        public async Task Should_GetByIdAsync()
        {
            var cancellationToken = TestContext.Current.CancellationToken;
            var userId = Guid.NewGuid();

            var repoMock = new Mock<ITaskRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var handler = new TaskService(repoMock.Object, unitOfWorkMock.Object);
            var id = Guid.NewGuid();
            var now = DateTime.UtcNow;
            var expectedTask = new TaskItem(id, userId, title: "Test", now);
            repoMock.Setup(r => r.GetByIdAsync(userId, id, cancellationToken)).ReturnsAsync(expectedTask);

            var result = await handler.GetByIdAsync(userId, id, cancellationToken);

            Assert.NotNull(result);
            Assert.IsType<TaskItem>(result);
            repoMock.Verify(r => r.GetByIdAsync(userId, It.IsAny<Guid>(), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task Should_GetAddAsync()
        {
            var cancellationToken = TestContext.Current.CancellationToken;
            var userId = Guid.NewGuid();

            var repoMock = new Mock<ITaskRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var handler = new TaskService(repoMock.Object, unitOfWorkMock.Object);

            var result = await handler.CreateAsync(userId, "hello", cancellationToken);

            Assert.NotNull(result);
            Assert.IsType<TaskItem>(result);
            repoMock.Verify(r => r.AddAsync(It.IsAny<TaskItem>(), cancellationToken), Times.Once);
            unitOfWorkMock.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Once);

        }

        [Fact]
        public async Task Should_MaskAsDoneAsync()
        {
            var cancellationToken = TestContext.Current.CancellationToken;
            var userId = Guid.NewGuid();

            var repoMock = new Mock<ITaskRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var handler = new TaskService(repoMock.Object, unitOfWorkMock.Object);
            var id = Guid.NewGuid();
            var now = DateTime.UtcNow;
            var expectedTask = new TaskItem(id, userId, title: "Test", now);
            expectedTask.MarkDone();

            repoMock.Setup(r => r.GetByIdAsync(userId, id, cancellationToken)).ReturnsAsync(expectedTask);

            await handler.MarkDoneAsync(userId, id, cancellationToken);

            repoMock.Verify(r => r.GetByIdAsync(userId, It.IsAny<Guid>(), cancellationToken), Times.Once);

            unitOfWorkMock.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Once);

            id = Guid.NewGuid();
            await Assert.ThrowsAsync<NotFoundException>(async () => await handler.MarkDoneAsync(userId, id, cancellationToken));

            repoMock.Verify(r => r.GetByIdAsync(userId, It.IsAny<Guid>(), cancellationToken), Times.Exactly(2));
        }

        [Fact]
        public async Task Should_DeleteAsync()
        {
            var cancellationToken = TestContext.Current.CancellationToken;
            var userId = Guid.NewGuid();

            var repoMock = new Mock<ITaskRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var handler = new TaskService(repoMock.Object, unitOfWorkMock.Object); ;
            var id = Guid.NewGuid();
            var now = DateTime.UtcNow;
            var expectedTask = new TaskItem(id, userId, title: "Test", now);
            expectedTask.MarkDone();

            repoMock.Setup(r => r.GetByIdAsync(userId, id, cancellationToken)).ReturnsAsync(expectedTask);

            await handler.DeleteAsync(userId, id, cancellationToken);

            repoMock.Verify(r => r.GetByIdAsync(userId, It.IsAny<Guid>(), cancellationToken), Times.Once);
            repoMock.Verify(r => r.DeleteAsync(userId, It.IsAny<Guid>(), cancellationToken), Times.Once);
            unitOfWorkMock.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Once);

            id = Guid.NewGuid();

            await Assert.ThrowsAsync<NotFoundException>(async () => await handler.DeleteAsync(userId, id, cancellationToken));

            repoMock.Verify(r => r.GetByIdAsync(userId, It.IsAny<Guid>(), cancellationToken), Times.Exactly(2));
            repoMock.Verify(r => r.DeleteAsync(userId, It.IsAny<Guid>(), cancellationToken), Times.Once);
        }
    }
}
