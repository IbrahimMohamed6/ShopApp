using ShopApp.BLL.DTOs;
using ShopApp.BLL.Mappers;
using ShopApp.BLL.Services.Interfaces;
using ShopApp.DAL.Models;
using ShopApp.DAL.Repositories.Interfaces;

namespace ShopApp.BLL.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repo;

        // Dependency injected via constructor
        public CategoryService(ICategoryRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllAsync()
        {
            var categories = await _repo.GetAllWithParentAsync();
            return categories.Select(c => c.ToDto());
        }

        public async Task<CategoryDto?> GetByIdAsync(int id)
        {
            var category = await _repo.GetByIdWithSubsAsync(id);
            return category?.ToDto();
        }

        public async Task<CategoryDto> CreateAsync(CategoryCreateDto dto)
        {
            var category = new Category
            {
                Name             = dto.Name,
                ParentCategoryId = dto.ParentCategoryId
            };
            await _repo.AddAsync(category);
            return category.ToDto();
        }

        public async Task UpdateAsync(CategoryUpdateDto dto)
        {
            var category = await _repo.GetByIdAsync(dto.CategoryId)
                ?? throw new KeyNotFoundException($"Category {dto.CategoryId} not found.");

            category.Name             = dto.Name;
            category.ParentCategoryId = dto.ParentCategoryId;

            await _repo.UpdateAsync(category);
        }

        public async Task DeleteAsync(int id)
        {
            await _repo.DeleteAsync(id);
        }
    }
}
