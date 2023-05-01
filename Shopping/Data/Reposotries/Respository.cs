using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Shopping.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Shopping.Data
{
    public abstract class Respository<TEntity> : IReposotory<TEntity>
        where TEntity : class, IEntity
    {
        protected ApplicationDbContext _context { set; get; }

        public Respository(ApplicationDbContext context)
        {
            _context = context;
        }

        //Get
        public async Task<List<TEntity>> GetAll(Expression<Func<TEntity, bool>> filter=null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
             Func<IQueryable<TEntity>, IGrouping<object, TEntity>> groupBy = null)
        {

            IQueryable<TEntity> result =  _context.Set<TEntity>().AsNoTracking().AsQueryable();
            if (filter != null)
            {
                result = result.Where(filter).AsQueryable();
            }
            if (groupBy != null)
            {
                result = groupBy(result).AsQueryable();
            }
            if (orderBy != null)
            {
                result = orderBy(result).AsQueryable();
            }

            return await result.ToListAsync();
        }
        public async Task<TEntity> Get(Guid? id)
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }
        public async Task<List<TEntity>> FindCondition(Expression<Func<TEntity, bool>> expression)
        {
            return await _context.Set<TEntity>().Where(expression).ToListAsync();
        }


        public virtual IQueryable<TEntity> Filter(Expression<Func<TEntity, bool>> filter,
                                           int skip = 0,
                                           int take = int.MaxValue,
                                           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                           Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null)
        {
            //where
            IQueryable<TEntity> result=(filter != null) ? _context.Set<TEntity>().AsNoTracking().
                Where(filter).AsQueryable() : _context.Set<TEntity>().AsNoTracking().AsQueryable();

            //include
            if (include != null)
            {
                result = include(result);
            }

            //order
            if (orderBy != null)
            {
                result = orderBy(result).AsQueryable();
            }

            //skip + take
            result = (skip == 0) ? result.Take(take) : result.Skip(skip).Take(take);

            return result.AsQueryable();
        }
        //DML
        public async Task<TEntity> Add(TEntity entity)
        {
            _context.Set<TEntity>().Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task<TEntity> Update(TEntity entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task<TEntity> Delete(Guid id)
        {
            var entity = await _context.Set<TEntity>().FindAsync(id);
            if (entity == null)
            {
                return entity;
            }

            _context.Set<TEntity>().Remove(entity);
            await _context.SaveChangesAsync();

            return entity;
        }

        public async Task<TEntity> Remove(TEntity entity)
        {
            _context.Entry(entity).State = EntityState.Deleted;
            await _context.SaveChangesAsync();
            return entity;
        }

        //Save
        public void Save() => _context.SaveChanges();

        public void Close() => _context.Dispose();

    }
}
