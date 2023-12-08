using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MusicStore.NetFramework.WebApp.Models;

namespace MusicStore.NetFramework.WebApp.ViewModels
{
	public class HomeViewModel
	{
		public IEnumerable<Album> Bestsellers { get; set; }
		public IEnumerable<Album> NewArrivals { get; set; }
		public IEnumerable<Genre> Genres { get; set; }
	}
}