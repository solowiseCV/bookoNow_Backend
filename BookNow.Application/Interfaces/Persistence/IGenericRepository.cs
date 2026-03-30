namespace BookNow.Application.Interfaces.Persistence;

public interface IGenericRepository<T>
    where T : class
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct);

    Task AddAsync(T entity, CancellationToken ct);
    void Update(T entity);
    void Remove(T entity);

    Task<bool> ExistsAsync(Guid id, CancellationToken ct);
    Task<int> CountAsync(CancellationToken ct);
}
