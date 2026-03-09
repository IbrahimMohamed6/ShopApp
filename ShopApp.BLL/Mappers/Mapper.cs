using ShopApp.BLL.DTOs;
using ShopApp.DAL.Models;

namespace ShopApp.BLL.Mappers
{
    /// <summary>
    /// Static mapping helpers — converts DAL entities → BLL DTOs.
    /// Keeps BLL decoupled from EF entities being passed up to PL.
    /// </summary>
    public static class Mapper
    {
        // ── Category ──────────────────────────────────────────────────────
        public static CategoryDto ToDto(this Category c) => new()
        {
            CategoryId       = c.CategoryId,
            Name             = c.Name,
            ParentCategoryId = c.ParentCategoryId,
            ParentName       = c.ParentCategory?.Name,
            SubCount         = c.SubCategories?.Count ?? 0
        };

        // ── Product ───────────────────────────────────────────────────────
        public static ProductDto ToDto(this Product p) => new()
        {
            ProductId     = p.ProductId,
            Name          = p.Name,
            SKU           = p.SKU,
            Price         = p.Price,
            StockQuantity = p.StockQuantity,
            IsActive      = p.IsActive,
            CategoryId    = p.CategoryId,
            CategoryName  = p.Category?.Name,
            Description   = p.Description,
            ImageUrl      = p.ImageUrl,
            CreatedAt     = p.CreatedAt
        };

        // ── Address ───────────────────────────────────────────────────────
        public static AddressDto ToDto(this Address a) => new()
        {
            AddressId = a.AddressId,
            UserId    = a.UserId,
            Country   = a.Country,
            City      = a.City,
            Street    = a.Street,
            Zip       = a.Zip,
            IsDefault = a.IsDefault
        };

        // ── OrderItem ─────────────────────────────────────────────────────
        public static OrderItemDto ToDto(this OrderItem oi) => new()
        {
            OrderItemId = oi.OrderItemId,
            ProductId   = oi.ProductId,
            ProductName = oi.Product?.Name ?? string.Empty,
            UnitPrice   = oi.UnitPrice,
            Quantity    = oi.Quantity,
            LineTotal   = oi.LineTotal
        };

        // ── Order ─────────────────────────────────────────────────────────
        public static OrderDto ToDto(this Order o) => new()
        {
            OrderId         = o.OrderId,
            OrderNumber     = o.OrderNumber,
            Status          = o.Status,
            OrderDate       = o.OrderDate,
            TotalAmount     = o.TotalAmount,
            UserId          = o.UserId,
            CustomerName    = o.User?.FullName,
            CustomerEmail   = o.User?.Email,
            ShippingAddress = o.ShippingAddress?.ToDto(),
            Items           = o.OrderItems?.Select(oi => oi.ToDto()) ?? Enumerable.Empty<OrderItemDto>()
        };
    }
}
