using ShopApp.DAL.Models;

namespace ShopApp.DAL.Repositories.Interfaces
{
    // ────────────────────────────────────────────────────────────────────────
    //  Generic base
    // ────────────────────────────────────────────────────────────────────────
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }

    // ────────────────────────────────────────────────────────────────────────
    //  Category
    // ────────────────────────────────────────────────────────────────────────
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<IEnumerable<Category>> GetAllWithParentAsync();
        Task<Category?> GetByIdWithSubsAsync(int id);
    }

    // ────────────────────────────────────────────────────────────────────────
    //  Product
    // ────────────────────────────────────────────────────────────────────────
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<(IEnumerable<Product> Items, int Total)> GetPagedAsync(
            int? categoryId, string? search, string? sort, int page, int pageSize);

        Task<Product?> GetByIdWithCategoryAsync(int id);
        Task DecreaseStockAsync(int productId, int qty);
        Task HardDeleteAsync(int id);
    }

    // ────────────────────────────────────────────────────────────────────────
    //  Address
    // ────────────────────────────────────────────────────────────────────────
    public interface IAddressRepository : IGenericRepository<Address>
    {
        Task<IEnumerable<Address>> GetByUserAsync(string userId);
    }

    // ────────────────────────────────────────────────────────────────────────
    //  Order
    // ────────────────────────────────────────────────────────────────────────
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<IEnumerable<Order>> GetByUserAsync(string userId);
        Task<IEnumerable<Order>> GetAllWithDetailsAsync();
        Task<Order?> GetByIdWithItemsAsync(int id);
        Task UpdateStatusAsync(int orderId, OrderStatus status);
    }
}
