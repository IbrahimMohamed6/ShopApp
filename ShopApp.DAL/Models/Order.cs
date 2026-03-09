using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopApp.DAL.Models
{
    public enum OrderStatus
    {
        Pending    = 0,
        Processing = 1,
        Shipped    = 2,
        Delivered  = 3,
        Cancelled  = 4
    }

    public class Order
    {
        public int         OrderId           { get; set; }

        [Required]
        public string      UserId            { get; set; } = string.Empty;

        public int         ShippingAddressId { get; set; }

        [Required, StringLength(50)]
        public string      OrderNumber       { get; set; } = string.Empty;

        public OrderStatus Status            { get; set; } = OrderStatus.Pending;
        public DateTime    OrderDate         { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "decimal(18,2)")]
        public decimal     TotalAmount       { get; set; }

        // Navigation
        public AppUser?              User            { get; set; }
        public Address?              ShippingAddress { get; set; }
        public ICollection<OrderItem> OrderItems     { get; set; } = new List<OrderItem>();
    }

    public class OrderItem
    {
        public int     OrderItemId { get; set; }
        public int     OrderId     { get; set; }
        public int     ProductId   { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice   { get; set; }

        public int     Quantity    { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal LineTotal   { get; set; }

        // Navigation
        public Order?   Order   { get; set; }
        public Product? Product { get; set; }
    }
}
