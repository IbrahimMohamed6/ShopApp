using ShopApp.BLL.DTOs;
using ShopApp.DAL.Models;

namespace ShopApp.BLL.Services.Interfaces
{
    // ────────────────────────────────────────────────────────────────────────
    //  ICategoryService
    // ────────────────────────────────────────────────────────────────────────
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllAsync();
        Task<CategoryDto?> GetByIdAsync(int id);
        Task<CategoryDto>  CreateAsync(CategoryCreateDto dto);
        Task UpdateAsync(CategoryUpdateDto dto);
        Task DeleteAsync(int id);
    }

    // ────────────────────────────────────────────────────────────────────────
    //  IProductService
    // ────────────────────────────────────────────────────────────────────────
    public interface IProductService
    {
        Task<ProductPagedResultDto> GetPagedAsync(
            int? categoryId, string? search, string? sort, int page, int pageSize);

        Task<ProductDto?> GetByIdAsync(int id);
        Task<ProductDto>  CreateAsync(ProductCreateDto dto);
        Task UpdateAsync(ProductUpdateDto dto);
        Task DeleteAsync(int id);           // soft-delete
    }

    // ────────────────────────────────────────────────────────────────────────
    //  IAddressService
    // ────────────────────────────────────────────────────────────────────────
    public interface IAddressService
    {
        Task<IEnumerable<AddressDto>> GetByUserAsync(string userId);
        Task<AddressDto?> GetByIdAsync(int id);
        Task<AddressDto>  CreateAsync(string userId, AddressCreateDto dto);
        Task DeleteAsync(int id);
    }

    // ────────────────────────────────────────────────────────────────────────
    //  ICartService  (session-based)
    // ────────────────────────────────────────────────────────────────────────
    public interface ICartService
    {
        CartDto  GetCart(Microsoft.AspNetCore.Http.HttpContext ctx);
        Task     AddItemAsync(Microsoft.AspNetCore.Http.HttpContext ctx, int productId, int qty = 1);
        void     UpdateItem(Microsoft.AspNetCore.Http.HttpContext ctx, int productId, int qty);
        void     RemoveItem(Microsoft.AspNetCore.Http.HttpContext ctx, int productId);
        void     ClearCart(Microsoft.AspNetCore.Http.HttpContext ctx);
    }

    // ────────────────────────────────────────────────────────────────────────
    //  IOrderService
    // ────────────────────────────────────────────────────────────────────────
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetByUserAsync(string userId);
        Task<IEnumerable<OrderDto>> GetAllAsync();
        Task<OrderDto?> GetByIdAsync(int id);
        Task<CheckoutResultDto> CheckoutAsync(string userId, CheckoutDto dto, CartDto cart);
        Task UpdateStatusAsync(int orderId, OrderStatus status);
    }
}
