using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShopApp.DAL.Data;
using ShopApp.DAL.Repositories;
using ShopApp.DAL.Repositories.Interfaces;

namespace ShopApp.DAL.Extensions
{
    /// <summary>
    /// Extension method to register all DAL dependencies.
    /// Called once from Program.cs:  builder.Services.AddDAL(configuration)
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddDAL(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // ── DbContext ────────────────────────────────────────────────
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("ShopApp.DAL")));

            // ── Repositories ─────────────────────────────────────────────
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IProductRepository,  ProductRepository>();
            services.AddScoped<IAddressRepository,  AddressRepository>();
            services.AddScoped<IOrderRepository,    OrderRepository>();

            return services;
        }
    }
}
