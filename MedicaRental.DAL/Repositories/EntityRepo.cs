using MedicaRental.DAL.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
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

            return Delete(entity);
        }

        public async Task<bool> DeleteManyById<TypeId>(IEnumerable<TypeId> ids)
        {
            List<TEntity> entities = new();

            foreach (var id in ids)
            {
                var entity = await _context.Set<TEntity>().FindAsync(id);
                
                if (entity is null) return false;
                entities.Add(entity);

            }

            return DeleteRange(entities);
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

        public async Task<IEnumerable<TEntity>> GetAllAsync(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, int? skip = null, int? take = null, bool disableTracking = true, bool ignoreQueryFilter = false)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();

            if (ignoreQueryFilter)
            {
                query = query.IgnoreQueryFilters();
            }

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include != null)
            {
                query = include(query);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if(skip != null)
            {
                query = query.Skip((int)skip);
            }

            if(take != null)
            {
                query = query.Take((int)take);
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, int? skip = null, int? take = null, bool disableTracking = true, bool ignoreQueryFilter = false)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();

            if (ignoreQueryFilter)
            {
                query = query.IgnoreQueryFilters();
            }

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (skip != null)
            {
                query = query.Skip((int)skip);
            }

            if (take != null)
            {
                query = query.Take((int)take);
            }

           return await query.ToListAsync();
        }

        public async Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, bool disableTracking = true, bool ignoreQueryFilter = false)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();

            if (ignoreQueryFilter)
            {
                query = query.IgnoreQueryFilters();
            }

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            query = query.Where(predicate);

            if (include != null)
            {
                query = include(query);
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, TResult>> selector, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, int? skip = null, int? take = null, bool disableTracking = true, bool ignoreQueryFilter = false)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();

            if (ignoreQueryFilter)
            {
                query = query.IgnoreQueryFilters();
            }

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include != null)
            {
                query = include(query);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (skip != null)
            {
                query = query.Skip((int)skip);
            }

            if (take != null)
            {
                query = query.Take((int)take);
            }

            return await query.Select(selector).ToListAsync();
        }

        public async Task<IEnumerable<TResult>> FindAllAsync<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, int? skip = null, int? take = null, bool disableTracking = true, bool ignoreQueryFilter = false)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();

            if (ignoreQueryFilter)
            {
                query = query.IgnoreQueryFilters();
            }

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (skip != null)
            {
                query = query.Skip((int)skip);
            }

            if (take != null)
            {
                query = query.Take((int)take);
            }

            return await query.Select(selector).ToListAsync();
        }

        public async Task<TResult?> FindAsync<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, bool disableTracking = true, bool ignoreQueryFilter = false)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();

            if (ignoreQueryFilter)
            {
                query = query.IgnoreQueryFilters();
            }

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return await query.Select(selector).FirstOrDefaultAsync();
        }

        public async Task<int> GetCountAsync(Expression<Func<TEntity, bool>>? predicate = null, bool ignoreQueryFilter = false)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();

            if (ignoreQueryFilter)
            {
                query = query.IgnoreQueryFilters();
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return await query.CountAsync();
        }
    }
}
