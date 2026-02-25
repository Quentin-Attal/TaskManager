using Domain.Exceptions;

namespace Domain.Entities;

public class TaskItem
{
    private TaskItem() { } // EF Core

    public TaskItem(Guid id, Guid userId, string title, DateTime createdAtUtc)
    {
        if (id == Guid.Empty) throw new DomainException("Task id cannot be empty.");
        if (userId == Guid.Empty) throw new DomainException("User id cannot be empty.");
        SetTitle(title);

        Id = id;
        UserId = userId;
        CreatedAtUtc = createdAtUtc;
        IsDone = false;
    }

    public Guid Id { get; private set; }
    public string Title { get; private set; } = "";
    public bool IsDone { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public Guid UserId { get; private set; }

    public AppUser? AppUser { get; private set; }

    public static TaskItem Create(Guid userId, string title, DateTime? nowUtc = null)
        => new(Guid.NewGuid(), userId, title, nowUtc ?? DateTime.UtcNow);

    public void SetTitle(string title)
    {
        title = (title ?? string.Empty).Trim();
        if (title.Length is < 1 or > 200)
            throw new DomainException("Task title must be between 1 and 200 characters.");

        Title = title;
    }

    public void MarkDone()
    {
        if (IsDone) return; // idempotent
        IsDone = true;
    }
}
