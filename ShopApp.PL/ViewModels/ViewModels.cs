using System.ComponentModel.DataAnnotations;
using ShopApp.BLL.DTOs;
using ShopApp.DAL.Models;

namespace ShopApp.PL.ViewModels
{
    // ────────────────────────────────────────────────────────────────────────
    //  Catalog
    // ────────────────────────────────────────────────────────────────────────
    public class CatalogIndexVM
    {
        public ProductPagedResultDto  PagedResult { get; set; } = new();
        public IEnumerable<CategoryDto> Categories { get; set; } = Enumerable.Empty<CategoryDto>();
        public int?    CategoryId { get; set; }
        public string? Search     { get; set; }
        public string? Sort       { get; set; }
        public int     Page       { get; set; } = 1;
    }

    public class ProductDetailsVM
    {
        public ProductDto              Product        { get; set; } = null!;
        public IEnumerable<ProductDto> RelatedProducts { get; set; } = Enumerable.Empty<ProductDto>();
    }

    // ────────────────────────────────────────────────────────────────────────
    //  Cart
    // ────────────────────────────────────────────────────────────────────────
    public class CartVM
    {
        public CartDto Cart { get; set; } = new();
    }

    // ────────────────────────────────────────────────────────────────────────
    //  Checkout
    // ────────────────────────────────────────────────────────────────────────
    public class CheckoutVM
    {
        public int?    SelectedAddressId { get; set; }
        public IEnumerable<AddressDto> SavedAddresses { get; set; } = Enumerable.Empty<AddressDto>();
        public CartDto Cart { get; set; } = new();

        // New address (optional)
        public string? NewCountry { get; set; }
        public string? NewCity    { get; set; }
        public string? NewStreet  { get; set; }
        public string? NewZip     { get; set; }
    }

    // ────────────────────────────────────────────────────────────────────────
    //  Orders
    // ────────────────────────────────────────────────────────────────────────
    public class OrderListVM
    {
        public IEnumerable<OrderDto> Orders { get; set; } = Enumerable.Empty<OrderDto>();
    }

    public class OrderDetailsVM
    {
        public OrderDto Order { get; set; } = null!;
    }

    // ────────────────────────────────────────────────────────────────────────
    //  Admin – Category
    // ────────────────────────────────────────────────────────────────────────
    public class AdminCategoryIndexVM
    {
        public IEnumerable<CategoryDto> Categories { get; set; } = Enumerable.Empty<CategoryDto>();
    }

    public class AdminCategoryFormVM
    {
        public int CategoryId { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public int? ParentCategoryId { get; set; }
        public IEnumerable<CategoryDto> AllCategories { get; set; } = Enumerable.Empty<CategoryDto>();
    }

    // ────────────────────────────────────────────────────────────────────────
    //  Admin – Product
    // ────────────────────────────────────────────────────────────────────────
    public class AdminProductIndexVM
    {
        public ProductPagedResultDto PagedResult { get; set; } = new();
        public IEnumerable<CategoryDto> Categories { get; set; } = Enumerable.Empty<CategoryDto>();
        public int?    CategoryId { get; set; }
        public string? Search     { get; set; }
        public string? Sort       { get; set; }
        public int     Page       { get; set; } = 1;
    }

    public class AdminProductFormVM
    {
        public int ProductId { get; set; }

        [Required, StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string SKU { get; set; } = string.Empty;

        [Required, Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required, Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }

        [Required(ErrorMessage = "Please select a category")]
        public int CategoryId { get; set; }

        public string?    Description { get; set; }
        public string?    ImageUrl    { get; set; }
        public IFormFile? ImageFile   { get; set; }
        public bool       IsActive    { get; set; } = true;

        public IEnumerable<CategoryDto> Categories { get; set; } = Enumerable.Empty<CategoryDto>();
    }

    // ────────────────────────────────────────────────────────────────────────
    //  Admin – Orders
    // ────────────────────────────────────────────────────────────────────────
    public class AdminOrderListVM
    {
        public IEnumerable<OrderDto> Orders      { get; set; } = Enumerable.Empty<OrderDto>();
        public OrderStatus?          FilterStatus { get; set; }
    }

    public class AdminOrderUpdateVM
    {
        public int         OrderId { get; set; }
        public OrderStatus Status  { get; set; }
    }
}
