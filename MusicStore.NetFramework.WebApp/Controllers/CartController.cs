using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MusicStore.NetFramework.WebApp.DAL;
using MusicStore.NetFramework.WebApp.Infrastructure;
using MusicStore.NetFramework.WebApp.ViewModels;

namespace MusicStore.NetFramework.WebApp.Controllers
{
	public class CartController : Controller
    {
		private ShoppingCartManager shoppingCartManager;
		private StoreContext db = new StoreContext();

		private ISessionManager sessionManager { get; set; }

        public CartController()
        {
			this.sessionManager = new SessionManager();
			this.shoppingCartManager = new ShoppingCartManager(this.sessionManager, this.db);
		}

        // GET: Cart
        public ActionResult Index()
        {
			var cartItems = shoppingCartManager.GetCart();
			var cartTotalPrice = shoppingCartManager.GetCartTotalPrice();
			CartViewModel cartVM = new CartViewModel() { CartItems = cartItems, TotalPrice = cartTotalPrice };
			return View(cartVM);
        }

		public ActionResult AddToCart(int id)
		{
			shoppingCartManager.AddToCart(id);

			//logger.Info("Added product {0} to cart", id);

			return RedirectToAction("Index", "Cart");
		}

		public int GetCartItemsCount()
		{
			return shoppingCartManager.GetCartItemsCount();
		}

		public ActionResult RemoveFromCart(int albumID)
		{
			ShoppingCartManager shoppingCartManager = new ShoppingCartManager(this.sessionManager, this.db);

			//int itemCount = shoppingCartManager.RemoveFromCart(albumID);
			//int cartItemsCount = shoppingCartManager.GetCartItemsCount();
			//decimal cartTotal = shoppingCartManager.GetCartTotalPrice();

			// Return JSON to process it in JavaScript
			// CartRemoveViewModel - klasa pomocnicza dla ajax do zwracania obiektu przez json 
			var result = new CartRemoveViewModel
			{
				//RemoveItemId = albumID,
				//RemovedItemCount = itemCount,
				//CartTotal = cartTotal,
				//CartItemsCount = cartItemsCount
				
				RemoveItemId = albumID,
				RemovedItemCount = shoppingCartManager.RemoveFromCart(albumID),
				CartItemsCount = shoppingCartManager.GetCartItemsCount(),
				CartTotal = shoppingCartManager.GetCartTotalPrice(),
			};

			return Json(result);
		}
	}
}