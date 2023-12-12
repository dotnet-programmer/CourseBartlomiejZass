using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using MusicStore.NetFramework.WebApp.App_Start;
using MusicStore.NetFramework.WebApp.DAL;
using MusicStore.NetFramework.WebApp.Infrastructure;
using MusicStore.NetFramework.WebApp.Models;
using MusicStore.NetFramework.WebApp.ViewModels;
using NLog;

namespace MusicStore.NetFramework.WebApp.Controllers
{
	public class CartController : Controller
	{
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
		private readonly StoreContext _context = new StoreContext();

		private readonly IMailService _mailService;
		private readonly ISessionManager _sessionManager;
		private readonly ShoppingCartManager _shoppingCartManager;
		private ApplicationUserManager _userManager;

		// INFO - Dependency Injection - NuGet Ninject wstrzyknie obiekt odpowiedniej klasy jako argument konstruktora
		public CartController(IMailService mailService, ISessionManager sessionManager)
		{
			_mailService = mailService;
			_sessionManager = sessionManager;
			_shoppingCartManager = new ShoppingCartManager(_sessionManager, _context);
		}

		public ApplicationUserManager UserManager
		{
			get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
			private set => _userManager = value;
		}

		// GET: Cart
		public ActionResult Index()
		{
			var cartItems = _shoppingCartManager.GetCart();
			var cartTotalPrice = _shoppingCartManager.GetCartTotalPrice();
			CartViewModel cartVM = new CartViewModel() { CartItems = cartItems, TotalPrice = cartTotalPrice };
			return View(cartVM);
		}

		public ActionResult AddToCart(int id)
		{
			_shoppingCartManager.AddToCart(id);
			_logger.Info("Added product {0} to cart", id);
			return RedirectToAction("Index", "Cart");
		}

		public ActionResult RemoveFromCart(int albumId)
		{
			// INFO - klasa pomocnicza dla ajax do zwracania obiektu przez json - CartRemoveViewModel
			var result = new CartRemoveViewModel
			{
				RemoveItemId = albumId,
				RemovedItemCount = _shoppingCartManager.RemoveFromCart(albumId),
				CartItemsCount = _shoppingCartManager.GetCartItemsCount(),
				CartTotal = _shoppingCartManager.GetCartTotalPrice(),
			};

			// Return JSON to process it in JavaScript
			return Json(result);
		}

		public async Task<ActionResult> Checkout()
		{
			// INFO - sprawdzenie czy użytkownik jest zalogowany - Request.IsAuthenticated
			if (Request.IsAuthenticated)
			{
				var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

				var order = new Order
				{
					FirstName = user.UserData.FirstName,
					LastName = user.UserData.LastName,
					Address = user.UserData.Address,
					CodeAndCity = user.UserData.CodeAndCity,
					Email = user.UserData.Email,
					PhoneNumber = user.UserData.PhoneNumber
				};

				return View(order);
			}
			else
			{
				return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Checkout", "Cart") });
			}
		}

		[HttpPost]
		public async Task<ActionResult> Checkout(Order orderDetails)
		{
			if (ModelState.IsValid)
			{
				_logger.Info("Checking out");

				// Get user
				var userId = User.Identity.GetUserId();

				// Save Order
				var newOrder = _shoppingCartManager.CreateOrder(orderDetails, userId);

				// Update profile information
				var user = await UserManager.FindByIdAsync(userId);

				// INFO - manualne wywołanie Model Bindera - TryUpdateModel
				TryUpdateModel(user.UserData);
				await UserManager.UpdateAsync(user);

				// Empty cart
				_shoppingCartManager.EmptyCart();

				var order = _context.Orders.Include("OrderItems").Include("OrderItems.Album").SingleOrDefault(o => o.OrderId == newOrder.OrderId);

				// Send mail confirmation
				//IMailService mailService = new HangFirePostalMailService();
				//mailService.SendOrderConfirmationEmail(order);

				_mailService.SendOrderConfirmationEmail(order);

				//string url = Url.Action("SendConfirmationEmail", "Cart", new { orderid = newOrder.OrderId, lastname = newOrder.LastName }, Request.Url.Scheme);

				// Hangfire - nice one (if ASP.NET app will be still running)
				//BackgroundJob.Enqueue(() => Helpers.CallUrl(url));

				// Strongly typed - without background
				//OrderConfirmationEmail email = new OrderConfirmationEmail();
				//email.To = order.Email;
				//email.Cost = order.TotalPrice;
				//email.OrderNumber = order.OrderId;
				//email.FullAddress = string.Format("{0} {1}, {2}, {3}", order.FirstName, order.LastName, order.Address, order.CodeAndCity);
				//email.OrderItems = order.OrderItems;
				//email.CoverPath = AppConfig.PhotosFolderRelative;

				// Loosely typed - without background
				//dynamic email = new Postal.Email("OrderConfirmation");
				//email.To = order.Email;
				//email.Cost = order.TotalPrice;
				//email.OrderNumber = order.OrderId;
				//email.FullAddress = string.Format("{0} {1}, {2}, {3}", order.FirstName, order.LastName, order.Address, order.CodeAndCity);
				//email.OrderItems = order.OrderItems;
				//email.CoverPath = AppConfig.PhotosFolderRelative;
				//email.Send();

				// Easiest background
				//HostingEnvironment.QueueBackgroundWorkItem(ct => email.Send());

				return RedirectToAction("OrderConfirmation");
			}
			else
			{
				return View(orderDetails);
			}
		}

		public ActionResult OrderConfirmation() => View();

		public int GetCartItemsCount() => _shoppingCartManager.GetCartItemsCount();
	}
}