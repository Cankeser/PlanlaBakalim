using PlanlaBakalim.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PlanlaBakalim.Service.Abstract
{
    public interface IService<T> where T : class
    {
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<T?> GetAsync(Expression<Func<T, bool>> expression);
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? expression = null);
        Task<T?> FindAsync(int id);
        IQueryable<T> Queryable();
        Task<int> SaveChangesAsync();
    }
}
