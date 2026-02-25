using Domain.Specification;
using Infrastructure.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.EFRepository;

public partial class EFRepository<T>(AppDbContext db) : IEFRepository<T> where T : class
{
    private readonly AppDbContext _db = db;

    private IQueryable<T> BuildQuery(QueryOptions options)
    {
        IQueryable<T> q = _db.Set<T>();

        if (options.AsNoTracking)
            q = q.AsNoTracking();

        if (options.AsSplitQuery)
            q = q.AsSplitQuery();

        return q;
    }

    private IQueryable<T> ApplySpecification(ISpecification<T> spec, QueryOptions options)
        => SpecificationEvaluator<T>.GetQuery(BuildQuery(options), spec);

}