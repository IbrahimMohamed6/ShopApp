using System.ComponentModel.DataAnnotations;

namespace ShopApp.DAL.Models
{
    public class Category
    {
        public int      CategoryId       { get; set; }

        [Required, StringLength(100)]
        public string   Name             { get; set; } = string.Empty;

        public int?     ParentCategoryId { get; set; }

        // Navigation
        public Category?            ParentCategory { get; set; }
        public ICollection<Category> SubCategories { get; set; } = new List<Category>();
        public ICollection<Product>  Products      { get; set; } = new List<Product>();
    }
}
