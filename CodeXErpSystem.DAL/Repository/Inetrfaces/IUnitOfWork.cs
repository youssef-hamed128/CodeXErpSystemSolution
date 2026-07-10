
using CodeXErpSystem.DAL.Entites;
using CodeXErpSystem.DAL.Repository.Inetrfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CodeXErpSystem.DAL.Repository.Inetrfaces
{
    public interface IUnitOfWork 
    {

        public IGenaricRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity, new();

        Task<int> CompleteAsync(CancellationToken ct = default);
        Task BeginTransactionAsync(CancellationToken ct = default);
        Task CommitTransactionAsync(CancellationToken ct = default);
        Task RollbackTransactionAsync(CancellationToken ct = default);
        public IInvoiceRepository InvoiceRepository { get; }
        public IProductRepository ProductRepository { get; }
    }
}