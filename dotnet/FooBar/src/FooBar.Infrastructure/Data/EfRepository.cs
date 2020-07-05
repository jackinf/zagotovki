using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FooBar.Domain.Entities;
using FooBar.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FooBar.Infrastructure.Data
{
    /// <summary>
    /// "There's some repetition here - couldn't we have some the sync methods call the async?"
    /// https://blogs.msdn.microsoft.com/pfxteam/2012/04/13/should-i-expose-synchronous-wrappers-for-asynchronous-methods/
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EfRepository<T> : IAsyncRepository<T> where T : BaseEntity, IAggregateRoot
    {
        // ReSharper disable once MemberCanBePrivate.Global
        protected readonly CatalogContext DbContext;

        public EfRepository(CatalogContext dbContext)
        {
            DbContext = dbContext;
        }

        public virtual async Task<T> GetByIdAsync(int id) 
            => await DbContext.Set<T>().FindAsync(id);

        public async Task<IReadOnlyList<T>> ListAllAsync() 
            => await DbContext.Set<T>().ToListAsync();

        public async Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec) 
            => await ApplySpecification(spec).ToListAsync();

        public async Task<int> CountAsync(ISpecification<T> spec) 
            => await ApplySpecification(spec).CountAsync();

        public async Task<T> AddAsync(T entity)
        {
            await DbContext.Set<T>().AddAsync(entity);
            await DbContext.SaveChangesAsync();

            return entity;
        }

        public async Task UpdateAsync(T entity)
        {
            DbContext.Entry(entity).State = EntityState.Modified;
            await DbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            DbContext.Set<T>().Remove(entity);
            await DbContext.SaveChangesAsync();
        }

        public async Task<T> FirstAsync(ISpecification<T> spec) 
            => await ApplySpecification(spec).FirstAsync();

        public async Task<T> FirstOrDefaultAsync(ISpecification<T> spec) 
            => await ApplySpecification(spec).FirstOrDefaultAsync();

        private IQueryable<T> ApplySpecification(ISpecification<T> spec) 
            => SpecificationEvaluator<T>.GetQuery(DbContext.Set<T>().AsQueryable(), spec);
    }
}