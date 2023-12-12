using System.Collections.Generic;
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