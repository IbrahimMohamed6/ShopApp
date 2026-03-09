using Microsoft.EntityFrameworkCore;
using ShopApp.BLL.DTOs;
using ShopApp.BLL.Mappers;
using ShopApp.BLL.Services.Interfaces;
using ShopApp.DAL.Data;
using ShopApp.DAL.Models;
using ShopApp.DAL.Repositories.Interfaces;

namespace ShopApp.BLL.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository   _orderRepo;
        private readonly IAddressRepository _addressRepo;
        private readonly ApplicationDbContext _db;   // needed for atomic transaction

        public OrderService(
            IOrderRepository   orderRepo,
            IAddressRepository addressRepo,
            ApplicationDbContext db)
        {
            _orderRepo   = orderRepo;
            _addressRepo = addressRepo;
            _db          = db;
        }

        public async Task<IEnumerable<OrderDto>> GetByUserAsync(string userId)
        {
            var orders = await _orderRepo.GetByUserAsync(userId);
            return orders.Select(o => o.ToDto());
        }

        public async Task<IEnumerable<OrderDto>> GetAllAsync()
        {
            var orders = await _orderRepo.GetAllWithDetailsAsync();
            return orders.Select(o => o.ToDto());
        }

        public async Task<OrderDto?> GetByIdAsync(int id)
        {
            var order = await _orderRepo.GetByIdWithItemsAsync(id);
            return order?.ToDto();
        }

        public async Task UpdateStatusAsync(int orderId, OrderStatus status)
        {
            await _orderRepo.UpdateStatusAsync(orderId, status);
        }

        /// <summary>
        /// Atomic checkout:
        ///  1. Resolve or create shipping address
        ///  2. Validate stock for all items
        ///  3. Create Order
        ///  4. Create OrderItems
        ///  5. Decrease product stock
        ///  6. Commit — rollback on any failure
        /// </summary>
        public async Task<CheckoutResultDto> CheckoutAsync(
            string userId, CheckoutDto dto, CartDto cart)
        {
            if (!cart.Items.Any())
                return Fail("Your cart is empty.");

            // ── 1. Resolve shipping address ──────────────────────────────
            int addressId;

            if (dto.SelectedAddressId.HasValue)
            {
                var addr = await _addressRepo.GetByIdAsync(dto.SelectedAddressId.Value);
                if (addr is null || addr.UserId != userId)
                    return Fail("Selected address is invalid.");
                addressId = addr.AddressId;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(dto.NewCountry) ||
                    string.IsNullOrWhiteSpace(dto.NewCity)    ||
                    string.IsNullOrWhiteSpace(dto.NewStreet)  ||
                    string.IsNullOrWhiteSpace(dto.NewZip))
                    return Fail("Please provide a complete shipping address.");

                var newAddr = new Address
                {
                    UserId  = userId,
                    Country = dto.NewCountry!,
                    City    = dto.NewCity!,
                    Street  = dto.NewStreet!,
                    Zip     = dto.NewZip!
                };
                await _addressRepo.AddAsync(newAddr);
                addressId = newAddr.AddressId;
            }

            // ── Atomic transaction ───────────────────────────────────────
            await using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                // 2. Validate stock
                foreach (var item in cart.Items)
                {
                    var product = await _db.Products.FindAsync(item.ProductId);
                    if (product is null || !product.IsActive || product.StockQuantity < item.Quantity)
                    {
                        await tx.RollbackAsync();
                        return Fail($"'{item.ProductName}' is unavailable or out of stock.");
                    }
                }

                // 3. Create Order
                var order = new Order
                {
                    UserId            = userId,
                    ShippingAddressId = addressId,
                    OrderNumber       = GenerateOrderNumber(),
                    Status            = OrderStatus.Pending,
                    OrderDate         = DateTime.UtcNow,
                    TotalAmount       = cart.Total
                };
                _db.Orders.Add(order);
                await _db.SaveChangesAsync();

                // 4. Create OrderItems
                foreach (var item in cart.Items)
                {
                    _db.OrderItems.Add(new OrderItem
                    {
                        OrderId   = order.OrderId,
                        ProductId = item.ProductId,
                        UnitPrice = item.UnitPrice,
                        Quantity  = item.Quantity,
                        LineTotal = item.LineTotal
                    });
                }

                // 5. Decrease stock
                foreach (var item in cart.Items)
                {
                    var product = await _db.Products.FindAsync(item.ProductId);
                    product!.StockQuantity -= item.Quantity;
                }

                await _db.SaveChangesAsync();
                await tx.CommitAsync();

                return new CheckoutResultDto
                {
                    Success     = true,
                    Message     = "Order placed successfully!",
                    OrderId     = order.OrderId,
                    OrderNumber = order.OrderNumber
                };
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return Fail($"Checkout failed: {ex.Message}");
            }
        }

        // ── Helpers ───────────────────────────────────────────────────────
        private static CheckoutResultDto Fail(string message) => new()
        {
            Success = false,
            Message = message
        };

        private static string GenerateOrderNumber() =>
            $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}-{Random.Shared.Next(1000, 9999)}";
    }
}
