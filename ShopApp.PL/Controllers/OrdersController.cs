using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShopApp.BLL.DTOs;
using ShopApp.BLL.Services.Interfaces;
using ShopApp.DAL.Models;
using ShopApp.PL.ViewModels;

namespace ShopApp.PL.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IOrderService   _orderService;
        private readonly IAddressService _addressService;
        private readonly ICartService    _cartService;
        private readonly UserManager<AppUser> _userManager;

        public OrdersController(
            IOrderService        orderService,
            IAddressService      addressService,
            ICartService         cartService,
            UserManager<AppUser> userManager)
        {
            _orderService   = orderService;
            _addressService = addressService;
            _cartService    = cartService;
            _userManager    = userManager;
        }

        // GET /Orders
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User)!;
            var orders = await _orderService.GetByUserAsync(userId);
            return View(new OrderListVM { Orders = orders });
        }

        // GET /Orders/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var order  = await _orderService.GetByIdAsync(id);

            if (order is null || order.UserId != userId) return NotFound();
            return View(new OrderDetailsVM { Order = order });
        }

        // GET /Orders/Checkout
        public async Task<IActionResult> Checkout()
        {
            var cart = _cartService.GetCart(HttpContext);
            if (!cart.Items.Any()) return RedirectToAction("Index", "Cart");

            var userId    = _userManager.GetUserId(User)!;
            var addresses = await _addressService.GetByUserAsync(userId);

            var vm = new CheckoutVM
            {
                Cart            = cart,
                SavedAddresses  = addresses,
                SelectedAddressId = addresses.FirstOrDefault(a => a.IsDefault)?.AddressId
            };
            return View(vm);
        }

        // POST /Orders/Checkout
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutVM vm)
        {
            var userId = _userManager.GetUserId(User)!;
            var cart   = _cartService.GetCart(HttpContext);

            var checkoutDto = new CheckoutDto
            {
                SelectedAddressId = vm.SelectedAddressId,
                NewCountry        = vm.NewCountry,
                NewCity           = vm.NewCity,
                NewStreet         = vm.NewStreet,
                NewZip            = vm.NewZip
            };

            // Delegate ALL business logic to BLL OrderService
            var result = await _orderService.CheckoutAsync(userId, checkoutDto, cart);

            if (!result.Success)
            {
                TempData["Error"] = result.Message;

                // Re-populate view model
                vm.Cart           = cart;
                vm.SavedAddresses = await _addressService.GetByUserAsync(userId);
                return View(vm);
            }

            _cartService.ClearCart(HttpContext);
            TempData["Success"] = $"Order {result.OrderNumber} placed successfully!";
            return RedirectToAction("Details", new { id = result.OrderId });
        }
    }
}
