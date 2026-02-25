using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Repositories
{
    public interface IUnitOfWork
    {
        Task SaveChangesAsync(CancellationToken ct);
        Task BeginTransactionAsync(CancellationToken ct);
        Task CommitTransactionAsync(CancellationToken ct);
        Task RollbackTransactionAsync(CancellationToken ct);
    }
}
