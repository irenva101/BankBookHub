using Client.Models;
using Common;
using Common.Models;
using Communication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using System.Diagnostics;

namespace Client.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DB _db;
        private static List<CartItem> _cart = new List<CartItem>();

        public HomeController(ILogger<HomeController> logger, DB db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Books()
        {
            IEnumerable<Common.Models.Book> books = _db.GetAllBooks();
            return View(books);
        }
        public IActionResult Balance()
        {
            return View();
        }
        public IActionResult Cart()
        {
            return View(_cart);
        }

        public IActionResult Buy()
        {

            //Made an proxy for our stateless interface service
            var statelessProxy = ServiceProxy.Create<IStatelessInterface>(new Uri("fabric:/BankBookHub/Validator"));
            var serviceName = statelessProxy.GetServiceDetails();
            return View("Index");
        }
        [HttpPost]
        public IActionResult AddToCart(int bookId, int quantity)
        {
            var book = _db.GetBookById(bookId);
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
