namespace Infrastructure.Repositories.Abstractions;

public interface IEFRepository<T> :
    IEFReadRepository<T>,
    IEFWriteRepository<T>
    where T : class
{
}