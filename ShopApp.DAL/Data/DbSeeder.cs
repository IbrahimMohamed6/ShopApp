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
                var men     = context.Categories.First(c => c.Name == "Men");
                var women   = context.Categories.First(c => c.Name == "Women");

                context.Products.AddRange(

                    // ── Phones ───────────────────────────────────────────
                    new Product { Name = "iPhone 15",              SKU = "PH-001", Price = 999m,  StockQuantity = 50, CategoryId = phones.CategoryId,  IsActive = true,  Description = "Apple iPhone 15 — 6.1\" OLED, A16 Bionic chip, 48 MP camera."                  },
                    new Product { Name = "iPhone 15 Pro",          SKU = "PH-002", Price = 1199m, StockQuantity = 35, CategoryId = phones.CategoryId,  IsActive = true,  Description = "Apple iPhone 15 Pro — titanium frame, A17 Pro chip, ProRes video."            },
                    new Product { Name = "Samsung Galaxy S24",     SKU = "PH-003", Price = 849m,  StockQuantity = 40, CategoryId = phones.CategoryId,  IsActive = true,  Description = "Samsung Galaxy S24 — 6.2\" Dynamic AMOLED, Snapdragon 8 Gen 3."              },
                    new Product { Name = "Samsung Galaxy S24 Ultra",SKU = "PH-004", Price = 1299m, StockQuantity = 25, CategoryId = phones.CategoryId, IsActive = true,  Description = "Samsung Galaxy S24 Ultra — 200 MP camera, built-in S Pen."                   },
                    new Product { Name = "Google Pixel 8",         SKU = "PH-005", Price = 699m,  StockQuantity = 30, CategoryId = phones.CategoryId,  IsActive = true,  Description = "Google Pixel 8 — Tensor G3 chip, 7 years of OS updates."                    },
                    new Product { Name = "OnePlus 12",             SKU = "PH-006", Price = 799m,  StockQuantity = 20, CategoryId = phones.CategoryId,  IsActive = true,  Description = "OnePlus 12 — Snapdragon 8 Gen 3, 100W fast charging."                       },

                    // ── Laptops ──────────────────────────────────────────
                    new Product { Name = "MacBook Pro 14\"",       SKU = "LP-001", Price = 1999m, StockQuantity = 20, CategoryId = laptops.CategoryId, IsActive = true,  Description = "Apple MacBook Pro 14 — M3 Pro chip, Liquid Retina XDR display."             },
                    new Product { Name = "MacBook Air 15\"",       SKU = "LP-002", Price = 1299m, StockQuantity = 25, CategoryId = laptops.CategoryId, IsActive = true,  Description = "Apple MacBook Air 15 — M2 chip, all-day battery life."                       },
                    new Product { Name = "Dell XPS 15",            SKU = "LP-003", Price = 1499m, StockQuantity = 30, CategoryId = laptops.CategoryId, IsActive = true,  Description = "Dell XPS 15 — Intel Core i7, 4K OLED touch display."                        },
                    new Product { Name = "HP Spectre x360 14",     SKU = "LP-004", Price = 1399m, StockQuantity = 18, CategoryId = laptops.CategoryId, IsActive = true,  Description = "HP Spectre x360 — 2-in-1 convertible, Intel Evo platform."                  },
                    new Product { Name = "Lenovo ThinkPad X1 Carbon",SKU = "LP-005", Price = 1349m,StockQuantity = 22, CategoryId = laptops.CategoryId,IsActive = true,  Description = "Lenovo ThinkPad X1 Carbon — ultra-light business laptop, 14\" IPS."         },
                    new Product { Name = "ASUS ROG Zephyrus G14", SKU = "LP-006", Price = 1599m, StockQuantity = 15, CategoryId = laptops.CategoryId, IsActive = true,  Description = "ASUS ROG Zephyrus G14 — AMD Ryzen 9, RTX 4060, 165 Hz display."             },

                    // ── Men ──────────────────────────────────────────────
                    new Product { Name = "Classic Slim-Fit Chinos", SKU = "MN-001", Price = 49m,  StockQuantity = 80, CategoryId = men.CategoryId,    IsActive = true,  Description = "Slim-fit chinos in stretch cotton — available in khaki, navy, and olive."   },
                    new Product { Name = "Oxford Button-Down Shirt",SKU = "MN-002", Price = 39m,  StockQuantity = 100,CategoryId = men.CategoryId,    IsActive = true,  Description = "Classic Oxford weave button-down shirt, regular fit."                        },
                    new Product { Name = "Merino Crew-Neck Sweater",SKU = "MN-003", Price = 79m,  StockQuantity = 60, CategoryId = men.CategoryId,    IsActive = true,  Description = "100% Merino wool crew-neck sweater — warm, lightweight, machine washable."},
                    new Product { Name = "Slim Denim Jeans",        SKU = "MN-004", Price = 59m,  StockQuantity = 90, CategoryId = men.CategoryId,    IsActive = true,  Description = "Slim-fit jeans in authentic indigo denim with slight stretch."             },
                    new Product { Name = "Hooded Puffer Jacket",    SKU = "MN-005", Price = 129m, StockQuantity = 45, CategoryId = men.CategoryId,    IsActive = true,  Description = "Lightweight hooded puffer jacket — water-resistant shell, warm fill."      },
                    new Product { Name = "Leather Derby Shoes",     SKU = "MN-006", Price = 119m, StockQuantity = 35, CategoryId = men.CategoryId,    IsActive = true,  Description = "Classic leather Derby shoes with rubber sole — black and brown."          },

                    // ── Women ────────────────────────────────────────────
                    new Product { Name = "Floral Wrap Dress",       SKU = "WM-001", Price = 65m,  StockQuantity = 70, CategoryId = women.CategoryId,  IsActive = true,  Description = "Elegant wrap dress in lightweight floral-print fabric."                     },
                    new Product { Name = "High-Rise Skinny Jeans",  SKU = "WM-002", Price = 59m,  StockQuantity = 85, CategoryId = women.CategoryId,  IsActive = true,  Description = "High-rise skinny jeans with 4-way stretch denim — super comfortable."     },
                    new Product { Name = "Cashmere Turtleneck",     SKU = "WM-003", Price = 99m,  StockQuantity = 50, CategoryId = women.CategoryId,  IsActive = true,  Description = "Soft cashmere turtleneck sweater — relaxed fit, available in 6 colors."   },
                    new Product { Name = "Tailored Blazer",         SKU = "WM-004", Price = 109m, StockQuantity = 40, CategoryId = women.CategoryId,  IsActive = true,  Description = "Fitted single-breasted blazer in stretch suiting fabric."                  },
                    new Product { Name = "Linen Wide-Leg Trousers", SKU = "WM-005", Price = 69m,  StockQuantity = 55, CategoryId = women.CategoryId,  IsActive = true,  Description = "Breezy wide-leg trousers in 100% linen — perfect for warm weather."       },
                    new Product { Name = "Leather Crossbody Bag",   SKU = "WM-006", Price = 89m,  StockQuantity = 30, CategoryId = women.CategoryId,  IsActive = true,  Description = "Compact pebbled-leather crossbody bag with adjustable strap."             }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}
