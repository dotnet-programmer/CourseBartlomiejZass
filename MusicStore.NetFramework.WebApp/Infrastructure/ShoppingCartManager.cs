using System;
using System.Collections.Generic;
using System.Linq;
using MusicStore.NetFramework.WebApp.DAL;
using MusicStore.NetFramework.WebApp.Models;

namespace MusicStore.NetFramework.WebApp.Infrastructure
{
	public class ShoppingCartManager
	{
		public const string CartSessionKey = "CartData";

		private readonly StoreContext _context;
		private readonly ISessionManager _sessionManager;

		public ShoppingCartManager(ISessionManager sessionManager, StoreContext context)
		{
			_sessionManager = sessionManager;
			_context = context;
		}

		public void AddToCart(int albumId)
		{
			var cart = GetCart();
			var cartItem = cart.Find(c => c.Album.AlbumId == albumId);

			if (cartItem != null)
			{
				cartItem.Quantity++;
			}
			else
			{
				// Find album and add it to cart
				var albumToAdd = _context.Albums
					.Where(a => a.AlbumId == albumId)
					.SingleOrDefault();

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

			_sessionManager.Set(CartSessionKey, cart);
		}

		public int RemoveFromCart(int albumId)
		{
			var cart = GetCart();
			var cartItem = cart.Find(c => c.Album.AlbumId == albumId);

			if (cartItem != null)
			{
				if (cartItem.Quantity > 1)
				{
					cartItem.Quantity--;
					return cartItem.Quantity;
				}
				else
				{
					cart.Remove(cartItem);
				}
			}

			// Return count of removed item currently inside cart
			return 0;
		}

		public List<CartItem> GetCart() => _sessionManager.Get<List<CartItem>>(CartSessionKey) ?? new List<CartItem>();

		public decimal GetCartTotalPrice() => GetCart().Sum(c => (c.Quantity * c.Album.Price));

		public int GetCartItemsCount() => GetCart().Sum(c => c.Quantity);

		public Order CreateOrder(Order newOrder, string userId)
		{
			newOrder.DateCreated = DateTime.Now;
			newOrder.UserId = userId;
			_context.Orders.Add(newOrder);

			if (newOrder.OrderItems == null)
			{
				newOrder.OrderItems = new List<OrderItem>();
			}

			decimal cartTotal = 0;
			var cart = GetCart();
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
			_context.SaveChanges();
			return newOrder;
		}

		public void EmptyCart() => _sessionManager.Set<List<CartItem>>(CartSessionKey, null);
	}
}