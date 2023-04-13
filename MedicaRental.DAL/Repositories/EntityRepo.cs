using MedicaRental.DAL.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.DAL.Repositories
{
    public abstract class EntityRepo<TEntity> : IEntityRepo<TEntity> where TEntity : class
    {
        private readonly MedicaRentalDbContext _context;

        public EntityRepo(MedicaRentalDbContext context)
        {
            _context = context;
        }

        public void AddAsync(TEntity entity)
        {
            _context.Set<TEntity>().AddAsync(entity);
        }

        public void AddRangeAsync(IEnumerable<TEntity> entities)
        {
            _context.Set<TEntity>().AddRangeAsync(entities);
        }

        public void Delete(TEntity entity)
        {
           _context.Set<TEntity>().Remove(entity);
        }

        public void DeleteRange(IEnumerable<TEntity> entities)
        {
           _context.Set<TEntity>().RemoveRange(entities);
        }

        public async Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, Expression<Func<TEntity, object>>? include = null)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();

            if (include != null)
            {
                query = query.Include(include);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            return await query.ToListAsync();
        }

        public Task<TEntity> FindAsync(object id, Expression<Func<TEntity, object>>? include = null)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, Expression<Func<TEntity, object>>? include = null)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> GetAsync(object id, Expression<Func<TEntity, object>>? include = null)
        {
            throw new NotImplementedException();
        }

        public void Update(TEntity entity)
        {
            _context.Update<TEntity>(entity);
        }

        public void UpdateRange(IEnumerable<TEntity> entities)
        {
            throw new NotImplementedException();

            //_context.UpdateRange<TEntity>(entities.ToArray());
        }
    }
}
