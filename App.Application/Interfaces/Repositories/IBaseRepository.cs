using App.Domain.Entities;

namespace App.Application.Interfaces.Repositories;

public interface IBaseRepository<TEntity> : IDisposable where TEntity : BaseEntity
{
    void Add(TEntity entity);

    void AddRange(IEnumerable<TEntity> entities);

    IQueryable<TEntity> GetAll();

    IQueryable<TEntity> GetAllNoTracking();

    Task<TEntity?> GetByIdAsync(Guid id);

    void Update(TEntity entity);

    Task<bool> ExistsAsync(int id);
    public void Remove(TEntity entity, bool hardDelete = false);
    void RemoveRange(IEnumerable<TEntity> entities, bool hardDelete = false);
}