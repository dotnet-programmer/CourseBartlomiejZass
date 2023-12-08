using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MusicStore.NetFramework.WebApp.DAL;
using MusicStore.NetFramework.WebApp.Models;
using MusicStore.NetFramework.WebApp.ViewModels;

namespace MusicStore.NetFramework.WebApp.Controllers
{
    public class HomeController : Controller
    {
		private StoreContext _storeContext = new StoreContext();

        // GET: Home
        public ActionResult Index()
        {
			var genres = _storeContext.Genres.ToList();

			var newArrivals = _storeContext.Albums
				.Where(x => x.IsHidden == false)
				.OrderByDescending(a=>a.DateAdded)
				.Take(3)
				.ToList();

			// OrderBy(x => Guid.NewGuid()) - zapewnia że dane będą w losowej kolejności
			var bestsellers = _storeContext.Albums
				.Where(x => x.IsHidden == false && x.IsBestseller)
				.OrderBy(x => Guid.NewGuid())
				.Take(3)
				.ToList();

			var vm = new HomeViewModel
			{
				Genres = genres,
				NewArrivals = newArrivals,
				Bestsellers = bestsellers,
			};

			return View(vm);
        }
		
		public ActionResult StaticContent(string viewName)
        {
            return View(viewName);
        }
    }
}