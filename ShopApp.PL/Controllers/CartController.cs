using Microsoft.AspNetCore.Mvc;
using ShopApp.BLL.Services.Interfaces;
using ShopApp.PL.ViewModels;

namespace ShopApp.PL.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        // GET /Cart
        public IActionResult Index()
        {
            var cart = _cartService.GetCart(HttpContext);
            return View(new CartVM { Cart = cart });
        }

        // POST /Cart/Add
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int productId, int qty = 1)
        {
            try
            {
                await _cartService.AddItemAsync(HttpContext, productId, qty);
                TempData["Success"] = "Product added to cart!";
            }
            catch (KeyNotFoundException)
            {
                TempData["Error"] = "Product not found.";
            }
            return RedirectToAction("Index");
        }

        // POST /Cart/Update
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Update(int productId, int qty)
        {
            _cartService.UpdateItem(HttpContext, productId, qty);
            return RedirectToAction("Index");
        }

        // POST /Cart/Remove
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Remove(int productId)
        {
            _cartService.RemoveItem(HttpContext, productId);
            TempData["Info"] = "Item removed from cart.";
            return RedirectToAction("Index");
        }

        // POST /Cart/Clear
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Clear()
        {
            _cartService.ClearCart(HttpContext);
            return RedirectToAction("Index");
        }
    }
}
