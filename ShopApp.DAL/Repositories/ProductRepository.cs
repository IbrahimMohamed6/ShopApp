using Microsoft.EntityFrameworkCore;
using ShopApp.DAL.Data;
using ShopApp.DAL.Models;
using ShopApp.DAL.Repositories.Interfaces;

namespace ShopApp.DAL.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext db) : base(db) { }

        public async Task<(IEnumerable<Product> Items, int Total)> GetPagedAsync(
            int? categoryId, string? search, string? sort, int page, int pageSize)
        {
            var query = _db.Products
                           .Include(p => p.Category)
                           .Where(p => p.IsActive)
                           .AsQueryable();

            if (categoryId.HasValue)
                query = query.Where(p =>
                    p.CategoryId == categoryId.Value ||
                    p.Category!.ParentCategoryId == categoryId.Value);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(p =>
                    p.Name.Contains(search) || p.SKU.Contains(search));

            query = sort switch
            {
                "price_asc"  => query.OrderBy(p => p.Price),
                "price_desc" => query.OrderByDescending(p => p.Price),
                "name"       => query.OrderBy(p => p.Name),
                _            => query.OrderByDescending(p => p.CreatedAt)
            };

            var total = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();
            return (items, total);
        }

        public async Task<Product?> GetByIdWithCategoryAsync(int id) =>
            await _db.Products
                     .Include(p => p.Category)
                     .FirstOrDefaultAsync(p => p.ProductId == id);

        public async Task DecreaseStockAsync(int productId, int qty)
        {
            var product = await _set.FindAsync(productId);
            if (product is not null)
            {
                product.StockQuantity -= qty;
                await _db.SaveChangesAsync();
            }
        }

        // Soft-delete: just mark inactive
        public override async Task DeleteAsync(int id)
        {
            var product = await _set.FindAsync(id);
            if (product is not null)
            {
                product.IsActive = false;
                await _db.SaveChangesAsync();
            }
        }
    }
}
