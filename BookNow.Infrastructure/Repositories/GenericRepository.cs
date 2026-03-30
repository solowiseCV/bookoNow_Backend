using BookNow.Application.Interfaces.Persistence;
using BookNow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookNow.Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T>
    where T : class
{
    protected readonly BookNowDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(BookNowDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken ct)
        => await _dbSet.FindAsync(new object[] { id }, ct);

    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct)
        => await _dbSet.AsNoTracking().ToListAsync(ct);

    public async Task AddAsync(T entity, CancellationToken ct)
        => await _dbSet.AddAsync(entity, ct);

    public void Update(T entity)
        => _dbSet.Update(entity);

    public void Remove(T entity)
        => _dbSet.Remove(entity);

    public async Task<bool> ExistsAsync(Guid id, CancellationToken ct)
        => await _dbSet.FindAsync(new object[] { id }, ct) != null;

    public async Task<int> CountAsync(CancellationToken ct)
        => await _dbSet.CountAsync(ct);
}
