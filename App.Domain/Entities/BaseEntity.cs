namespace App.Domain.Entities;

public abstract class BaseEntity
{
    public int Id { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? DeletedAt { get; private set; }
}