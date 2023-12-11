using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MusicStore.NetFramework.WebApp.Models;

namespace MusicStore.NetFramework.WebApp.ViewModels
{
	public class CartViewModel
	{
		public List<CartItem> CartItems { get; set; }
		public decimal TotalPrice { get; set; }
	}
}