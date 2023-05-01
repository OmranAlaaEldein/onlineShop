using Microsoft.EntityFrameworkCore.Query;
using Shopping.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Shopping.Data
{
    public interface IReposotory<T> where T : class, IEntity
    {
        //Get
        Task<List<T>> GetAll(Expression<Func<T, bool>> filter=null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IGrouping<object,T>> groupBy = null);

        Task<T> Get(Guid? id);

        Task<List<T>> FindCondition(Expression<Func<T, bool>> expression);

        IQueryable<T> Filter(Expression<Func<T, bool>> filter,
                                          int skip = 0,
                                          int take = int.MaxValue,
                                          Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                          Func<IQueryable<T>, IIncludableQueryable<T, object>> include=null);


        //DML
        Task<T> Add(T entity);
        
        Task<T> Update(T entity);
        
        Task<T> Delete(Guid id);

        Task<T> Remove(T entity);
        //Save

        void Save();
        void Close();
    }
}
