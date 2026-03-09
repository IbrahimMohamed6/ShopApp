using System.ComponentModel.DataAnnotations;

namespace ShopApp.DAL.Models
{
    public class Address
    {
        public int AddressId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string Country   { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string City      { get; set; } = string.Empty;

        [Required, StringLength(200)]
        public string Street { get; set; } = string.Empty;

        [Required, StringLength(20)]
        public string Zip { get; set; } = string.Empty;

        public bool   IsDefault { get; set; }

        // Navigation
        public AppUser? User   { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
