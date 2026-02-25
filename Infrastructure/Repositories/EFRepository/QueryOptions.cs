namespace Infrastructure.Repositories.EFRepository;

public sealed record QueryOptions(
    bool AsNoTracking = false,
    bool AsSplitQuery = false
)
{
    public static readonly QueryOptions Default = new();
    public static readonly QueryOptions ReadOnly = new(AsNoTracking: true);
    public static readonly QueryOptions ReadOnlySplit = new(AsNoTracking: true, AsSplitQuery: true);
    public static readonly QueryOptions SplitOnly = new(AsSplitQuery: true);
}