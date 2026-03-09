using Microsoft.EntityFrameworkCore;
using ShopApp.DAL.Data;
using ShopApp.DAL.Models;
using ShopApp.DAL.Repositories.Interfaces;

namespace ShopApp.DAL.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(ApplicationDbContext db) : base(db) { }

        public async Task<IEnumerable<Order>> GetByUserAsync(string userId) =>
            await _db.Orders
                     .Include(o => o.ShippingAddress)
                     .Where(o => o.UserId == userId)
                     .OrderByDescending(o => o.OrderDate)
                     .ToListAsync();

        public async Task<IEnumerable<Order>> GetAllWithDetailsAsync() =>
            await _db.Orders
                     .Include(o => o.User)
                     .Include(o => o.ShippingAddress)
                     .OrderByDescending(o => o.OrderDate)
                     .ToListAsync();

        public async Task<Order?> GetByIdWithItemsAsync(int id) =>
            await _db.Orders
                     .Include(o => o.User)
                     .Include(o => o.ShippingAddress)
                     .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                     .FirstOrDefaultAsync(o => o.OrderId == id);

        public async Task UpdateStatusAsync(int orderId, OrderStatus status)
        {
            var order = await _set.FindAsync(orderId);
            if (order is not null)
            {
                order.Status = status;
                await _db.SaveChangesAsync();
            }
        }
    }
}
