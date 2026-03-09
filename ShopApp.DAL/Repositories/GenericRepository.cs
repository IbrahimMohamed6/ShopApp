using Microsoft.EntityFrameworkCore;
using ShopApp.DAL.Data;
using ShopApp.DAL.Repositories.Interfaces;

namespace ShopApp.DAL.Repositories
{
    /// <summary>
    /// Generic repository that provides default CRUD for any entity.
    /// Concrete repositories inherit this and add their own specialized queries.
    /// </summary>
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _db;
        protected readonly DbSet<T> _set;

        public GenericRepository(ApplicationDbContext db)
        {
            _db  = db;
            _set = db.Set<T>();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync() =>
            await _set.ToListAsync();

        public virtual async Task<T?> GetByIdAsync(int id) =>
            await _set.FindAsync(id);

        public virtual async Task AddAsync(T entity)
        {
            await _set.AddAsync(entity);
            await _db.SaveChangesAsync();
        }

        public virtual async Task UpdateAsync(T entity)
        {
            _set.Update(entity);
            await _db.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(int id)
        {
            var entity = await _set.FindAsync(id);
            if (entity is not null)
            {
                _set.Remove(entity);
                await _db.SaveChangesAsync();
            }
        }

        public virtual async Task<bool> ExistsAsync(int id) =>
            await _set.FindAsync(id) is not null;
    }
}
