using Persistence.GenericRepository.Base;
using Persistence.GenericRepository.Repository;

namespace Persistence.GenericRepository.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<TEntity, TId> GetRepository<TEntity, TId>() where TEntity : BaseEntity<TId>;

        Task<int> CommitAsync(CancellationToken cancellationToken = default);
    }
}
