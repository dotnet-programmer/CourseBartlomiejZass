using System.Collections.Generic;
using MusicStore.NetFramework.WebApp.Models;

namespace MusicStore.NetFramework.WebApp.ViewModels
{
	public class CartViewModel
	{
		public List<CartItem> CartItems { get; set; }
		public decimal TotalPrice { get; set; }
	}
}