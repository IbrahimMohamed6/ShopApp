using System.Text.Json;
using Microsoft.AspNetCore.Http;
using ShopApp.BLL.DTOs;
using ShopApp.BLL.Services.Interfaces;
using ShopApp.DAL.Repositories.Interfaces;

namespace ShopApp.BLL.Services
{
    public class CartService : ICartService
    {
        private const string CartKey = "ShoppingCart";
        private readonly IProductRepository _productRepo;

        public CartService(IProductRepository productRepo)
        {
            _productRepo = productRepo;
        }

        public CartDto GetCart(HttpContext ctx)
        {
            var json = ctx.Session.GetString(CartKey);
            return json is null
                ? new CartDto()
                : JsonSerializer.Deserialize<CartDto>(json)!;
        }

        public async Task AddItemAsync(HttpContext ctx, int productId, int qty = 1)
        {
            var product = await _productRepo.GetByIdWithCategoryAsync(productId)
                          ?? throw new KeyNotFoundException($"Product {productId} not found.");

            var cart = GetCart(ctx);
            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);

            if (item is not null)
                item.Quantity += qty;
            else
                cart.Items.Add(new CartItemDto
                {
                    ProductId   = product.ProductId,
                    ProductName = product.Name,
                    UnitPrice   = product.Price,
                    Quantity    = qty,
                    ImageUrl    = product.ImageUrl
                });

            Save(ctx, cart);
        }

        public void UpdateItem(HttpContext ctx, int productId, int qty)
        {
            var cart = GetCart(ctx);
            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (item is null) return;

            if (qty <= 0) cart.Items.Remove(item);
            else          item.Quantity = qty;

            Save(ctx, cart);
        }

        public void RemoveItem(HttpContext ctx, int productId)
        {
            var cart = GetCart(ctx);
            cart.Items.RemoveAll(i => i.ProductId == productId);
            Save(ctx, cart);
        }

        public void ClearCart(HttpContext ctx) =>
            ctx.Session.Remove(CartKey);

        // ── private ──────────────────────────────────────────────────────
        private static void Save(HttpContext ctx, CartDto cart) =>
            ctx.Session.SetString(CartKey, JsonSerializer.Serialize(cart));
    }
}
