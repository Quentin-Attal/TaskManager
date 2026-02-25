namespace Infrastructure.Repositories.Abstractions;

public interface IEFRepository<T> :
    IEFReadRepository<T>,
    IEFWriteRepository<T>,
    IEFUnitOfWorkRepository,
    IAsyncDisposable
    where T : class
{
}