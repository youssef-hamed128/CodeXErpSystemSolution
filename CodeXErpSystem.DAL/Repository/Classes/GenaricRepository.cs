using CodeXErpSystem.DAL.Contexts;
using CodeXErpSystem.DAL.Entites;
using CodeXErpSystem.DAL.Repository.Inetrfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CodeXErpSystem.DAL.Repository.Classes
{
    
    public class GenaricRepository<TEntity> : IGenaricRepository<TEntity> where TEntity : BaseEntity, new()
    {
        private readonly CodeXDbContext dbContext;

        public GenaricRepository(CodeXDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<IEnumerable<TEntity>> GetAll(bool IsTracked, CancellationToken ct = default)
        {
           var items = IsTracked ? dbContext.Set<TEntity>() : dbContext.Set<TEntity>().AsNoTracking();
           return await items.ToListAsync(ct);
        }
        public async Task<TEntity?> GetById(int Id, CancellationToken ct = default)
        {
            var item = dbContext.Set<TEntity>().FirstOrDefaultAsync(p=>p.Id == Id , ct);
            return await item;
        }
        public void Add(TEntity item)
        {
            dbContext.Set<TEntity>().Add(item);

        }
        public void Update(TEntity item)
        {
            dbContext.Set<TEntity>().Update(item);
        }
        public void Delete(int id)
        {
            var item = dbContext.Set<TEntity>().FirstOrDefaultAsync(item => item.Id == id);
            if(item is not null)
                dbContext.Set<TEntity>().Remove(item.Result);
        }
        public async Task<int> CompleteAsync()
        {
            return await dbContext.SaveChangesAsync();
        }
        public async
            Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, bool isTracked = false, CancellationToken ct = default)
        {
            var items = isTracked ? dbContext.Set<TEntity>() : dbContext.Set<TEntity>().AsNoTracking();
            return await items.FirstOrDefaultAsync(predicate, ct);

        }
        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default)
        {
            return await dbContext.Set<TEntity>().AnyAsync(predicate, ct);
        }

    }
}
