using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopApp.DAL.Models
{
    public class Product
    {
        public int     ProductId     { get; set; }
        public int     CategoryId    { get; set; }

        [Required, StringLength(200)]
        public string  Name          { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string  SKU           { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price         { get; set; }

        public int     StockQuantity { get; set; }
        public bool    IsActive      { get; set; } = true;
        public DateTime CreatedAt   { get; set; } = DateTime.UtcNow;

        public string? Description  { get; set; }
        public string? ImageUrl     { get; set; }

        // Navigation
        public Category?  Category   { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
