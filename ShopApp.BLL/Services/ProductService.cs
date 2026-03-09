using ShopApp.BLL.DTOs;
using ShopApp.BLL.Mappers;
using ShopApp.BLL.Services.Interfaces;
using ShopApp.DAL.Models;
using ShopApp.DAL.Repositories.Interfaces;

namespace ShopApp.BLL.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repo;

        public ProductService(IProductRepository repo)
        {
            _repo = repo;
        }

        public async Task<ProductPagedResultDto> GetPagedAsync(
            int? categoryId, string? search, string? sort, int page, int pageSize)
        {
            var (items, total) = await _repo.GetPagedAsync(categoryId, search, sort, page, pageSize);

            return new ProductPagedResultDto
            {
                Items      = items.Select(p => p.ToDto()),
                TotalItems = total,
                Page       = page,
                PageSize   = pageSize
            };
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var product = await _repo.GetByIdWithCategoryAsync(id);
            return product?.ToDto();
        }

        public async Task<ProductDto> CreateAsync(ProductCreateDto dto)
        {
            var product = new Product
            {
                Name          = dto.Name,
                SKU           = dto.SKU,
                Price         = dto.Price,
                StockQuantity = dto.StockQuantity,
                CategoryId    = dto.CategoryId,
                Description   = dto.Description,
                ImageUrl      = dto.ImageUrl,
                IsActive      = dto.IsActive
            };
            await _repo.AddAsync(product);
            return product.ToDto();
        }

        public async Task UpdateAsync(ProductUpdateDto dto)
        {
            var product = await _repo.GetByIdAsync(dto.ProductId)
                ?? throw new KeyNotFoundException($"Product {dto.ProductId} not found.");

            product.Name          = dto.Name;
            product.SKU           = dto.SKU;
            product.Price         = dto.Price;
            product.StockQuantity = dto.StockQuantity;
            product.CategoryId    = dto.CategoryId;
            product.Description   = dto.Description;
            product.ImageUrl      = dto.ImageUrl;
            product.IsActive      = dto.IsActive;

            await _repo.UpdateAsync(product);
        }

        public async Task DeleteAsync(int id)
        {
            await _repo.DeleteAsync(id); // soft-delete in ProductRepository
        }
    }
}
