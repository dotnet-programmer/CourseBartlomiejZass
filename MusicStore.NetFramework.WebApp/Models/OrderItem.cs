﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicStore.NetFramework.WebApp.Models
{
	public class OrderItem
	{
		public int OrderItemId { get; set; }
		public int Quantity { get; set; }
		public decimal UnitPrice { get; set; }

		public int AlbumId { get; set; }
		public Album Album { get; set; }

		public int OrderId { get; set; }
		public Order Order { get; set; }
	}
}