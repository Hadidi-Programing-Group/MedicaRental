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
        public Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null,
                                           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
                                           Expression<Func<TEntity, object>>? include = null);

        public Task<TEntity> GetAsync(object id, Expression<Func<TEntity, object>>? include = null);

        public Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>>? predicate = null,
                                           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
                                           Expression<Func<TEntity, object>>? include = null);
        public Task<TEntity> FindAsync(object id, Expression<Func<TEntity, object>>? include = null);

        public void AddAsync(TEntity entity);
        public void AddRangeAsync(IEnumerable<TEntity> entities);
        
        public void Delete(TEntity entity);
        public void DeleteRange(IEnumerable<TEntity> entities);
        
        public void Update(TEntity entity);
        public void UpdateRange(IEnumerable<TEntity> entities);
    }
}
