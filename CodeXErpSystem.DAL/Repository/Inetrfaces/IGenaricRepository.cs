using CodeXErpSystem.DAL.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CodeXErpSystem.DAL.Repository.Inetrfaces
{
    public interface IGenaricRepository <TEntity> where TEntity : BaseEntity,new()
    {
        Task<IEnumerable<TEntity>> GetAll(bool IsTracked, CancellationToken ct = default);
        Task<TEntity?> GetById(int Id, CancellationToken ct = default);
        void Add(TEntity item);
        void Update(TEntity item);
        void Delete(int id);
        Task<int> CompleteAsync();
        Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, bool isTracked = false, CancellationToken ct = default);
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default);
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> filter = null , string includeProperties = null , Func<IQueryable<TEntity> ,IOrderedQueryable<TEntity>> orderBy = null , bool isTracked = true , CancellationToken ct = default);



    }
}
