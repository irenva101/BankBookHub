using Client.Models;
using Common;
using Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using System.Diagnostics;
using Validator;

namespace Client.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private static List<CartItem> _cart = new List<CartItem>();
        private readonly DB _db;

        public HomeController(ILogger<HomeController> logger, DB db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            ViewBag.AccountBalance = _db.AccountBalance;
            ViewBag.BookCount = _db.Books.Count;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Books()
        {
            return View(DB.GetDB().Books);
        }
        public IActionResult Balance()
        {
            return View();
        }
        public IActionResult Cart()
        {
            return View(_cart);
        }

        public async Task<IActionResult> Buy()
        {
            if (_cart == null || !_cart.Any())
            {
                TempData["Message"] = "Cart is empty or not initialized.";
                return RedirectToAction("Index");
            }

            var proxy = ServiceProxy.Create<IValidator>(new Uri("fabric:/BankBookHub/Validator"));
            var result = await proxy.ValidateRequest(_cart);

            if (result)
            {
                TempData["Message"] = "Transaction successfully completed!";
            }
            else
            {
                TempData["Message"] = "Transaction failed due to insufficient funds or nonexistent books.";
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult AddToCart(int bookId, int quantity)
        {
            var book = DB.GetDB().GetBookById(bookId);
            var cartItem = _cart.FirstOrDefault(c => c.Book.Id == bookId);
            if (cartItem == null)
            {
                _cart.Add(new CartItem { Book = book, Quantity = quantity });
            }
            else
            {
                cartItem.Quantity += quantity;
            }
            return Json(new { success = true });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
