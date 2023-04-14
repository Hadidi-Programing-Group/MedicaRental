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
                                           Expression<Func<TEntity, object>>? include = null);

        public Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>>? include = null);

        public Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>>? predicate = null,
                                           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
                                           Expression<Func<TEntity, object>>? include = null);

        public Task AddAsync(TEntity entity);
        public Task AddRangeAsync(IEnumerable<TEntity> entities);

        public Task DeleteById(TEntity entity);
        public void Delete(TEntity entity);
        public void DeleteRange(IEnumerable<TEntity> entities);
        
        public void Update(TEntity entity);
        public void UpdateRange(IEnumerable<TEntity> entities);
    }
}
