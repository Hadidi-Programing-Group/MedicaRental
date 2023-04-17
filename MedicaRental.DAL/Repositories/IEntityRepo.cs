using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.DAL.Repositories
{
    public interface IEntityRepo<TEntity> where TEntity : class
    {
        public Task<IEnumerable<TEntity>> GetAllAsync(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
                                          Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, int? skip = null, int? take = null, bool disableTracking = true);

        public Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, bool disableTracking = true);

        public Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>>? predicate = null,
                                           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
                                           Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, int? skip = null, int? take = null, bool disableTracking = true);

        public Task<IEnumerable<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, TResult>> selector, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
                                          Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, int? skip = null, int? take = null, bool disableTracking = true);

        public Task<TResult?> FindAsync<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, bool disableTracking = true);

        public Task<IEnumerable<TResult>> FindAllAsync<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>>? predicate = null,
                                           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
                                           Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, int? skip = null, int? take = null, bool disableTracking = true);

        public Task<int> GetCountAsync(Expression<Func<TEntity, bool>>? predicate = null);

        public Task<bool> AddAsync(TEntity entity);
        public Task<bool> AddRangeAsync(IEnumerable<TEntity> entities);

        public Task<bool> DeleteOneById<TypeId>(TypeId id);
        public Task<bool> DeleteManyById<TypeId>(IEnumerable<TypeId> ids);
        public bool Delete(TEntity entity);
        public bool DeleteRange(IEnumerable<TEntity> entities);
        
        public bool Update(TEntity entity);
        public bool UpdateRange(IEnumerable<TEntity> entities);
    }
}
