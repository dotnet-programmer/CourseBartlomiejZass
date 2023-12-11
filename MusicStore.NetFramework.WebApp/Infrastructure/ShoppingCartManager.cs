using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MusicStore.NetFramework.WebApp.DAL;
using MusicStore.NetFramework.WebApp.Models;

namespace MusicStore.NetFramework.WebApp.Infrastructure
{
	public class ShoppingCartManager
	{
		public const string CartSessionKey = "CartData";

		private StoreContext db;
		private ISessionManager session;

		public ShoppingCartManager(ISessionManager session, StoreContext db)
		{
			this.session = session;
			this.db = db;
		}

		public void AddToCart(int albumid)
		{
			var cart = this.GetCart();

			var cartItem = cart.Find(c => c.Album.AlbumId == albumid);

			if (cartItem != null)
				cartItem.Quantity++;
			else
			{
				// Find album and add it to cart
				var albumToAdd = db.Albums.Where(a => a.AlbumId == albumid).SingleOrDefault();
				if (albumToAdd != null)
				{
					var newCartItem = new CartItem()
					{
						Album = albumToAdd,
						Quantity = 1,
						TotalPrice = albumToAdd.Price
					};

					cart.Add(newCartItem);
				}
			}

			session.Set(CartSessionKey, cart);
		}

		public List<CartItem> GetCart()
		{
			List<CartItem> cart;

			if (session.Get<List<CartItem>>(CartSessionKey) == null)
			{
				cart = new List<CartItem>();
			}
			else
			{
				cart = session.Get<List<CartItem>>(CartSessionKey) as List<CartItem>;
			}

			return cart;
		}

		public int RemoveFromCart(int albumid)
		{
			var cart = this.GetCart();

			var cartItem = cart.Find(c => c.Album.AlbumId == albumid);

			if (cartItem != null)
			{
				if (cartItem.Quantity > 1)
				{
					cartItem.Quantity--;
				}
				else
				{
					cart.Remove(cartItem);
				}
			}

			// Return count of removed item currently inside cart
			return cartItem.Quantity;
		}

		public decimal GetCartTotalPrice()
		{
			var cart = this.GetCart();
			return cart.Sum(c => (c.Quantity * c.Album.Price));
		}

		public int GetCartItemsCount()
		{
			var cart = this.GetCart();
			int count = cart.Sum(c => c.Quantity);

			return count;
		}

		public Order CreateOrder(Order newOrder, string userId)
		{
			var cart = this.GetCart();

			newOrder.DateCreated = DateTime.Now;
			newOrder.UserId = userId;

			this.db.Orders.Add(newOrder);

			if (newOrder.OrderItems == null)
				newOrder.OrderItems = new List<OrderItem>();

			decimal cartTotal = 0;

			foreach (var cartItem in cart)
			{
				var newOrderItem = new OrderItem()
				{
					AlbumId = cartItem.Album.AlbumId,
					Quantity = cartItem.Quantity,
					UnitPrice = cartItem.Album.Price
				};

				cartTotal += (cartItem.Quantity * cartItem.Album.Price);

				newOrder.OrderItems.Add(newOrderItem);
			}

			newOrder.TotalPrice = cartTotal;

			this.db.SaveChanges();

			return newOrder;
		}

		public void EmptyCart()
		{
			session.Set<List<CartItem>>(CartSessionKey, null);
		}
	}
}