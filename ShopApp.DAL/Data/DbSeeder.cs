using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using ShopApp.DAL.Data;
using ShopApp.DAL.Models;

namespace ShopApp.DAL.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<AppUser>>();
            var context     = services.GetRequiredService<ApplicationDbContext>();

            // ── Roles ────────────────────────────────────────────────────
            foreach (var role in new[] { "Admin", "Customer" })
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));

            // ── Default Admin ────────────────────────────────────────────
            if (await userManager.FindByEmailAsync("admin@shop.com") == null)
            {
                var admin = new AppUser
                {
                    UserName = "admin@shop.com",
                    Email    = "admin@shop.com",
                    FullName = "Shop Admin",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(admin, "Admin@123");
                await userManager.AddToRoleAsync(admin, "Admin");
            }

            // ── Categories ───────────────────────────────────────────────
            if (!context.Categories.Any())
            {
                var electronics = new Category { Name = "Electronics" };
                var clothing    = new Category { Name = "Clothing"    };
                context.Categories.AddRange(electronics, clothing);
                await context.SaveChangesAsync();

                context.Categories.AddRange(
                    new Category { Name = "Phones",  ParentCategoryId = electronics.CategoryId },
                    new Category { Name = "Laptops", ParentCategoryId = electronics.CategoryId },
                    new Category { Name = "Men",     ParentCategoryId = clothing.CategoryId    },
                    new Category { Name = "Women",   ParentCategoryId = clothing.CategoryId    }
                );
                await context.SaveChangesAsync();
            }

            // ── Products ─────────────────────────────────────────────────
            if (!context.Products.Any())
            {
                var phones  = context.Categories.First(c => c.Name == "Phones");
                var laptops = context.Categories.First(c => c.Name == "Laptops");

                context.Products.AddRange(
                    new Product { Name = "iPhone 15",         SKU = "PH-001", Price = 999m,  StockQuantity = 50, CategoryId = phones.CategoryId,  IsActive = true, Description = "Apple iPhone 15"        },
                    new Product { Name = "Samsung Galaxy S24", SKU = "PH-002", Price = 849m,  StockQuantity = 40, CategoryId = phones.CategoryId,  IsActive = true, Description = "Samsung flagship"        },
                    new Product { Name = "MacBook Pro 14",     SKU = "LP-001", Price = 1999m, StockQuantity = 20, CategoryId = laptops.CategoryId, IsActive = true, Description = "Apple MacBook Pro"       },
                    new Product { Name = "Dell XPS 15",        SKU = "LP-002", Price = 1499m, StockQuantity = 30, CategoryId = laptops.CategoryId, IsActive = true, Description = "Dell XPS flagship"       }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}
