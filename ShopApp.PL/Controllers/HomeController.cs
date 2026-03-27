using Microsoft.AspNetCore.Mvc;

namespace ShopApp.PL.Controllers
{
    public class HomeController : Controller
    {
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // Pick up the status code if it came from UseStatusCodePagesWithReExecute
            var statusCode = HttpContext.Response.StatusCode;
            if (statusCode == 404)
            {
                ViewData["ErrorMessage"] = "The page you are looking for does not exist.";
            }

            return View();
        }
    }
}
