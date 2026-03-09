using Microsoft.Extensions.DependencyInjection;
using ShopApp.BLL.Services;
using ShopApp.BLL.Services.Interfaces;

namespace ShopApp.BLL.Extensions
{
    /// <summary>
    /// Extension method to register all BLL dependencies.
    /// Called once from Program.cs:  builder.Services.AddBLL()
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddBLL(this IServiceCollection services)
        {
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductService,  ProductService>();
            services.AddScoped<IAddressService,  AddressService>();
            services.AddScoped<IOrderService,    OrderService>();
            services.AddScoped<ICartService,     CartService>();

            return services;
        }
    }
}
