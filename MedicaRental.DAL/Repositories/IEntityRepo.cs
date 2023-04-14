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
                                           Expression<Func<TEntity, object>>[]? includes = null);

        public Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>>[]? includes = null);

        public Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>>? predicate = null,
                                           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
                                           Expression<Func<TEntity, object>>[]? includes = null);

        public Task<bool> AddAsync(TEntity entity);
        public Task<bool> AddRangeAsync(IEnumerable<TEntity> entities);

        public Task<bool> DeleteOneById<TypeId>(TypeId id);
        public Task<List<TypeId>> DeleteManyById<TypeId>(IEnumerable<TypeId> ids);
        public bool Delete(TEntity entity);
        public bool DeleteRange(IEnumerable<TEntity> entities);
        
        public bool Update(TEntity entity);
        public bool UpdateRange(IEnumerable<TEntity> entities);
    }
}
