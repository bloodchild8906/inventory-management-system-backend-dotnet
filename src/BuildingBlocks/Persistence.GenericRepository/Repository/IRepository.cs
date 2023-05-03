using Persistence.GenericRepository.Base;
using System.Linq.Expressions;

namespace Persistence.GenericRepository.Repository
{

    public interface IRepository<TEntity, TId> where TEntity : BaseEntity<TId> 
    {
        Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
        Task<TEntity?> GetByAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default);
        Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<List<TEntity>> GetAllByAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default);
        Task<int> CountAsync(CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default);
        Task<TEntity> CreateAndSaveAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task UpdateAndSaveAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task DeleteAndSaveAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
        void Edit(TEntity entity);
        void Remove(TEntity entity);
        IQueryable<TEntity> All { get; }
    }
}
