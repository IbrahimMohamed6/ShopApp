using System.ComponentModel.DataAnnotations;
using ShopApp.DAL.Models;

namespace ShopApp.BLL.DTOs
{
    // ────────────────────────────────────────────────────────────────────────
    //  Category DTOs
    // ────────────────────────────────────────────────────────────────────────
    public class CategoryDto
    {
        public int     CategoryId       { get; set; }
        public string  Name             { get; set; } = string.Empty;
        public int?    ParentCategoryId { get; set; }
        public string? ParentName       { get; set; }
        public int     SubCount         { get; set; }
    }

    public class CategoryCreateDto
    {
        [Required, StringLength(100)]
        public string Name             { get; set; } = string.Empty;
        public int?   ParentCategoryId { get; set; }
    }

    public class CategoryUpdateDto : CategoryCreateDto
    {
        public int CategoryId { get; set; }
    }

    // ────────────────────────────────────────────────────────────────────────
    //  Product DTOs
    // ────────────────────────────────────────────────────────────────────────
    public class ProductDto
    {
        public int     ProductId     { get; set; }
        public string  Name          { get; set; } = string.Empty;
        public string  SKU           { get; set; } = string.Empty;
        public decimal Price         { get; set; }
        public int     StockQuantity { get; set; }
        public bool    IsActive      { get; set; }
        public int     CategoryId    { get; set; }
        public string? CategoryName  { get; set; }
        public string? Description   { get; set; }
        public string? ImageUrl      { get; set; }
        public DateTime CreatedAt   { get; set; }
    }

    public class ProductCreateDto
    {
        [Required, StringLength(200)]
        public string  Name          { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string  SKU           { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue)]
        public decimal Price         { get; set; }

        [Range(0, int.MaxValue)]
        public int     StockQuantity { get; set; }

        [Required]
        public int     CategoryId    { get; set; }

        public string? Description   { get; set; }
        public string? ImageUrl      { get; set; }
        public bool    IsActive      { get; set; } = true;
    }

    public class ProductUpdateDto : ProductCreateDto
    {
        public int ProductId { get; set; }
    }

    public class ProductPagedResultDto
    {
        public IEnumerable<ProductDto> Items      { get; set; } = Enumerable.Empty<ProductDto>();
        public int                     TotalItems { get; set; }
        public int                     Page       { get; set; }
        public int                     PageSize   { get; set; }
        public int                     TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
    }

    // ────────────────────────────────────────────────────────────────────────
    //  Address DTOs
    // ────────────────────────────────────────────────────────────────────────
    public class AddressDto
    {
        public int    AddressId { get; set; }
        public string UserId    { get; set; } = string.Empty;
        public string Country   { get; set; } = string.Empty;
        public string City      { get; set; } = string.Empty;
        public string Street    { get; set; } = string.Empty;
        public string Zip       { get; set; } = string.Empty;
        public bool   IsDefault { get; set; }

        public string FullAddress => $"{Street}, {City}, {Country} {Zip}";
    }

    public class AddressCreateDto
    {
        [Required, StringLength(100)] public string Country { get; set; } = string.Empty;
        [Required, StringLength(100)] public string City    { get; set; } = string.Empty;
        [Required, StringLength(200)] public string Street  { get; set; } = string.Empty;
        [Required, StringLength(20)]  public string Zip     { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
    }

    // ────────────────────────────────────────────────────────────────────────
    //  Cart DTOs
    // ────────────────────────────────────────────────────────────────────────
    public class CartItemDto
    {
        public int     ProductId   { get; set; }
        public string  ProductName { get; set; } = string.Empty;
        public decimal UnitPrice   { get; set; }
        public int     Quantity    { get; set; }
        public string? ImageUrl    { get; set; }
        public decimal LineTotal   => UnitPrice * Quantity;
    }

    public class CartDto
    {
        public List<CartItemDto> Items     { get; set; } = new();
        public decimal           Total     => Items.Sum(i => i.LineTotal);
        public int               ItemCount => Items.Sum(i => i.Quantity);
    }

    // ────────────────────────────────────────────────────────────────────────
    //  Order DTOs
    // ────────────────────────────────────────────────────────────────────────
    public class OrderItemDto
    {
        public int     OrderItemId  { get; set; }
        public int     ProductId    { get; set; }
        public string  ProductName  { get; set; } = string.Empty;
        public decimal UnitPrice    { get; set; }
        public int     Quantity     { get; set; }
        public decimal LineTotal    { get; set; }
    }

    public class OrderDto
    {
        public int          OrderId           { get; set; }
        public string       OrderNumber       { get; set; } = string.Empty;
        public OrderStatus  Status            { get; set; }
        public DateTime     OrderDate         { get; set; }
        public decimal      TotalAmount       { get; set; }
        public string       UserId            { get; set; } = string.Empty;
        public string?      CustomerName      { get; set; }
        public string?      CustomerEmail     { get; set; }
        public AddressDto?  ShippingAddress   { get; set; }
        public IEnumerable<OrderItemDto> Items { get; set; } = Enumerable.Empty<OrderItemDto>();
    }

    public class CheckoutDto
    {
        public int?   SelectedAddressId { get; set; }

        // Optional new address
        public string? NewCountry { get; set; }
        public string? NewCity    { get; set; }
        public string? NewStreet  { get; set; }
        public string? NewZip     { get; set; }
    }

    public class CheckoutResultDto
    {
        public bool   Success      { get; set; }
        public string Message      { get; set; } = string.Empty;
        public int?   OrderId      { get; set; }
        public string OrderNumber  { get; set; } = string.Empty;
    }
}
