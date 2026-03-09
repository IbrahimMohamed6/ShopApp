using Microsoft.AspNetCore.Mvc;
using ShopApp.BLL.Services.Interfaces;
using ShopApp.PL.ViewModels;

namespace ShopApp.PL.Controllers
{
    public class CatalogController : Controller
    {
        // ── Injected BLL services (no DAL references here) ───────────────
        private readonly IProductService  _productService;
        private readonly ICategoryService _categoryService;

        public CatalogController(
            IProductService  productService,
            ICategoryService categoryService)
        {
            _productService  = productService;
            _categoryService = categoryService;
        }

        // GET /Catalog  or  GET /
        public async Task<IActionResult> Index(
            int? categoryId, string? q, string? sort, int page = 1)
        {
            const int pageSize = 12;
            var paged      = await _productService.GetPagedAsync(categoryId, q, sort, page, pageSize);
            var categories = await _categoryService.GetAllAsync();

            var vm = new CatalogIndexVM
            {
                PagedResult = paged,
                Categories  = categories,
                CategoryId  = categoryId,
                Search      = q,
                Sort        = sort,
                Page        = page
            };
            return View(vm);
        }

        // GET /Catalog/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product is null) return NotFound();

            // Fetch a few related products from same category
            var related = await _productService.GetPagedAsync(
                product.CategoryId, null, null, 1, 5);

            var vm = new ProductDetailsVM
            {
                Product         = product,
                RelatedProducts = related.Items.Where(p => p.ProductId != id)
            };
            return View(vm);
        }
    }
}
