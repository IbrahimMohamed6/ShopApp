using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopApp.BLL.DTOs;
using ShopApp.BLL.Services.Interfaces;
using ShopApp.DAL.Models;
using ShopApp.PL.ViewModels;

namespace ShopApp.PL.Areas.Admin.Controllers
{
    // ────────────────────────────────────────────────────────────────────────
    //  Admin / Categories
    // ────────────────────────────────────────────────────────────────────────
    [Area("Admin"), Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _categoryService.GetAllAsync();
            return View(new AdminCategoryIndexVM { Categories = list });
        }

        public async Task<IActionResult> Create()
        {
            var vm = new AdminCategoryFormVM
            {
                AllCategories = await _categoryService.GetAllAsync()
            };
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminCategoryFormVM vm)
        {
            if (!ModelState.IsValid)
            {
                vm.AllCategories = await _categoryService.GetAllAsync();
                return View(vm);
            }
            await _categoryService.CreateAsync(new CategoryCreateDto
            {
                Name             = vm.Name,
                ParentCategoryId = vm.ParentCategoryId
            });
            TempData["Success"] = "Category created.";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var cat = await _categoryService.GetByIdAsync(id);
            if (cat is null) return NotFound();

            var vm = new AdminCategoryFormVM
            {
                CategoryId       = cat.CategoryId,
                Name             = cat.Name,
                ParentCategoryId = cat.ParentCategoryId,
                AllCategories    = (await _categoryService.GetAllAsync())
                                   .Where(c => c.CategoryId != id)
            };
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AdminCategoryFormVM vm)
        {
            if (!ModelState.IsValid)
            {
                vm.AllCategories = (await _categoryService.GetAllAsync())
                                   .Where(c => c.CategoryId != vm.CategoryId);
                return View(vm);
            }
            await _categoryService.UpdateAsync(new CategoryUpdateDto
            {
                CategoryId       = vm.CategoryId,
                Name             = vm.Name,
                ParentCategoryId = vm.ParentCategoryId
            });
            TempData["Success"] = "Category updated.";
            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _categoryService.DeleteAsync(id);
            TempData["Success"] = "Category deleted.";
            return RedirectToAction("Index");
        }
    }

    // ────────────────────────────────────────────────────────────────────────
    //  Admin / Products
    // ────────────────────────────────────────────────────────────────────────
    [Area("Admin"), Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private readonly IProductService  _productService;
        private readonly ICategoryService _categoryService;

        public ProductsController(
            IProductService  productService,
            ICategoryService categoryService)
        {
            _productService  = productService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _productService.GetPagedAsync(null, null, null, 1, 200);
            return View(new AdminProductIndexVM { Products = result.Items });
        }

        public async Task<IActionResult> Create()
        {
            var vm = new AdminProductFormVM
            {
                Categories = await _categoryService.GetAllAsync()
            };
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminProductFormVM vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Categories = await _categoryService.GetAllAsync();
                return View(vm);
            }
            await _productService.CreateAsync(new ProductCreateDto
            {
                Name          = vm.Name,
                SKU           = vm.SKU,
                Price         = vm.Price,
                StockQuantity = vm.StockQuantity,
                CategoryId    = vm.CategoryId,
                Description   = vm.Description,
                ImageUrl      = vm.ImageUrl,
                IsActive      = vm.IsActive
            });
            TempData["Success"] = "Product created.";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product is null) return NotFound();

            var vm = new AdminProductFormVM
            {
                ProductId     = product.ProductId,
                Name          = product.Name,
                SKU           = product.SKU,
                Price         = product.Price,
                StockQuantity = product.StockQuantity,
                CategoryId    = product.CategoryId,
                Description   = product.Description,
                ImageUrl      = product.ImageUrl,
                IsActive      = product.IsActive,
                Categories    = await _categoryService.GetAllAsync()
            };
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AdminProductFormVM vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Categories = await _categoryService.GetAllAsync();
                return View(vm);
            }
            await _productService.UpdateAsync(new ProductUpdateDto
            {
                ProductId     = vm.ProductId,
                Name          = vm.Name,
                SKU           = vm.SKU,
                Price         = vm.Price,
                StockQuantity = vm.StockQuantity,
                CategoryId    = vm.CategoryId,
                Description   = vm.Description,
                ImageUrl      = vm.ImageUrl,
                IsActive      = vm.IsActive
            });
            TempData["Success"] = "Product updated.";
            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _productService.DeleteAsync(id);
            TempData["Success"] = "Product deactivated.";
            return RedirectToAction("Index");
        }
    }

    // ────────────────────────────────────────────────────────────────────────
    //  Admin / Orders
    // ────────────────────────────────────────────────────────────────────────
    [Area("Admin"), Authorize(Roles = "Admin")]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<IActionResult> Index(OrderStatus? status)
        {
            var orders = await _orderService.GetAllAsync();
            if (status.HasValue)
                orders = orders.Where(o => o.Status == status.Value);

            return View(new AdminOrderListVM
            {
                Orders       = orders,
                FilterStatus = status
            });
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order is null) return NotFound();
            return View(new OrderDetailsVM { Order = order });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(AdminOrderUpdateVM vm)
        {
            await _orderService.UpdateStatusAsync(vm.OrderId, vm.Status);
            TempData["Success"] = "Order status updated.";
            return RedirectToAction("Details", new { id = vm.OrderId });
        }
    }
}
