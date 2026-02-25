
using FluentAssertions;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Abstractions;
using Moq;

namespace UnitTests
{
    public class BaseRepositoryTests
    {
        public sealed class DummyEntity { }
        public sealed class TestRepository(IEFRepository<BaseRepositoryTests.DummyEntity> crud) : BaseRepository<DummyEntity>(crud)
        {
        }

        [Fact]
        public async Task AddAsync_Should_Call_Crud_AddAsync()
        {
            var ct = new CancellationTokenSource().Token;
            var entity = new DummyEntity();

            var crud = new Mock<IEFRepository<DummyEntity>>(MockBehavior.Strict);
            crud.Setup(x => x.AddAsync(entity, ct)).ReturnsAsync(entity);

            var repo = new TestRepository(crud.Object);

            var result = await repo.AddAsync(entity, ct);

            result.Should().BeSameAs(entity);
            crud.Verify(x => x.AddAsync(entity, ct), Times.Once);
            crud.VerifyNoOtherCalls();
        }

        [Fact]
        public void Update_Should_Call_Crud_Update()
        {
            var ct = new CancellationTokenSource().Token;
            var entity = new DummyEntity();

            var crud = new Mock<IEFRepository<DummyEntity>>(MockBehavior.Strict);
            crud.Setup(x => x.Update(entity));

            var repo = new TestRepository(crud.Object);

            repo.Update(entity);

            crud.Verify(x => x.Update(entity), Times.Once);
            crud.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task DeleteAsync_Guid_Should_Call_Crud_DeleteAsync()
        {
            var ct = CancellationToken.None;
            var id = Guid.NewGuid();

            var crud = new Mock<IEFRepository<DummyEntity>>(MockBehavior.Strict);
            crud.Setup(x => x.DeleteAsync(id, ct)).Returns(Task.CompletedTask);

            var repo = new TestRepository(crud.Object);

            await repo.DeleteAsync(id, ct);

            crud.Verify(x => x.DeleteAsync(id, ct), Times.Once);
            crud.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task DeleteAsync_id_Should_Call_Crud_DeleteAsync()
        {
            var ct = CancellationToken.None;
            var id = 20;

            var crud = new Mock<IEFRepository<DummyEntity>>(MockBehavior.Strict);
            crud.Setup(x => x.DeleteAsync(id, ct)).Returns(Task.CompletedTask);

            var repo = new TestRepository(crud.Object);

            await repo.DeleteAsync(id, ct);

            crud.Verify(x => x.DeleteAsync(id, ct), Times.Once);
            crud.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task GetByIdAsync_id_Should_Call_Crud_GetByIdAsync()
        {
            var ct = CancellationToken.None;
            var expected = new DummyEntity();
            var id = 20;

            var crud = new Mock<IEFRepository<DummyEntity>>(MockBehavior.Strict);
            crud.Setup(x => x.GetByIdAsync(id, ct)).ReturnsAsync(expected);

            var repo = new TestRepository(crud.Object);

            var result = await repo.GetByIdAsync(id, ct);

            result.Should().BeSameAs(expected);
            crud.Verify(x => x.GetByIdAsync(id, ct), Times.Once);
            crud.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task GetByIdAsync_guid_Should_Call_Crud_GetByIdAsync()
        {
            var ct = CancellationToken.None;
            var expected = new DummyEntity();
            var id = Guid.NewGuid();

            var crud = new Mock<IEFRepository<DummyEntity>>(MockBehavior.Strict);
            crud.Setup(x => x.GetByIdAsync(id, ct)).ReturnsAsync(expected);

            var repo = new TestRepository(crud.Object);

            var result = await repo.GetByIdAsync(id, ct);

            result.Should().BeSameAs(expected);
            crud.Verify(x => x.GetByIdAsync(id, ct), Times.Once);
            crud.VerifyNoOtherCalls();
        }
    }
}
