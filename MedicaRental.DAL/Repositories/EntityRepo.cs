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

        public async Task<bool> AddAsync(TEntity entity)
        {
            try
            {
                await _context.Set<TEntity>().AddAsync(entity);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> AddRangeAsync(IEnumerable<TEntity> entities)
        {
            try
            {
                await _context.Set<TEntity>().AddRangeAsync(entities);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteOneById<TypeId>(TypeId id)
        {
            var entity = await _context.Set<TEntity>().FindAsync(id);
            if (entity is null)
                return false;

            Delete(entity);
            return true;
        }

        public async Task<List<TypeId>> DeleteManyById<TypeId>(IEnumerable<TypeId> ids)
        {
            List<TypeId> failedIds = new();

            foreach (var id in ids)
            {
                var res = await DeleteOneById(id);

                if (!res) failedIds.Add(id);
            }

            return failedIds;
        }

        public bool Delete(TEntity entity)
        {
            try
            {
                _context.Set<TEntity>().Remove(entity);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool DeleteRange(IEnumerable<TEntity> entities)
        {
            try
            {
                _context.Set<TEntity>().RemoveRange(entities);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, Expression<Func<TEntity, object>>[]? includes = null)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
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

        public async Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>>[]? includes = null)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();

            query = query.Where(predicate);

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, Expression<Func<TEntity, object>>[]? includes = null)
        {

            IQueryable<TEntity> query = _context.Set<TEntity>();

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            return await query.ToListAsync();
        }

        public bool Update(TEntity entity)
        {
            try
            {
                _context.Update(entity);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdateRange(IEnumerable<TEntity> entities)
        {
            try
            {
                _context.UpdateRange(entities);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
