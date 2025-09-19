using Microsoft.EntityFrameworkCore;
using PlanlaBakalim.Data;
using PlanlaBakalim.Service.Abstract;
using System.Linq.Expressions;

namespace PlanlaBakalim.Service.Concrete
{
    public class Service<T> : IService<T> where T : class
    {

        internal  DatabaseContext _db;
        internal DbSet<T> _dbSet;
        public Service(DatabaseContext db)
        {
            _db = db;
            _dbSet = _db.Set<T>();
        }
        public void Add(T entity) => _dbSet.Add(entity);
        public void Update(T entity) => _dbSet.Update(entity);
        public void Delete(T entity) => _dbSet.Remove(entity);

        public async Task<T?> FindAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? expression = null)
        {
            return await (expression == null ? _dbSet.AsNoTracking().ToListAsync() : _dbSet.Where(expression).AsNoTracking().ToListAsync());
        }

        public async Task<T?> GetAsync(Expression<Func<T, bool>> expression)
        {
            return await _dbSet.FirstOrDefaultAsync(expression);
        }

        public IQueryable<T> Queryable()
        {
            return _dbSet;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _db.SaveChangesAsync();
        }


    }
}
