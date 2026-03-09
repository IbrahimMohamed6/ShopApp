using Microsoft.EntityFrameworkCore;
using ShopApp.DAL.Data;
using ShopApp.DAL.Models;
using ShopApp.DAL.Repositories.Interfaces;

namespace ShopApp.DAL.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext db) : base(db) { }

        public async Task<IEnumerable<Category>> GetAllWithParentAsync() =>
            await _db.Categories
                     .Include(c => c.ParentCategory)
                     .Include(c => c.SubCategories)
                     .OrderBy(c => c.Name)
                     .ToListAsync();

        public async Task<Category?> GetByIdWithSubsAsync(int id) =>
            await _db.Categories
                     .Include(c => c.SubCategories)
                     .Include(c => c.ParentCategory)
                     .FirstOrDefaultAsync(c => c.CategoryId == id);

        // Override GetAllAsync to always include parent info
        public override async Task<IEnumerable<Category>> GetAllAsync() =>
            await GetAllWithParentAsync();
    }
}
