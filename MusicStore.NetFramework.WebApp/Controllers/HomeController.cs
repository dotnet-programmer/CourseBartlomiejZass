using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MusicStore.NetFramework.WebApp.DAL;
using MusicStore.NetFramework.WebApp.Infrastructure;
using MusicStore.NetFramework.WebApp.Models;
using MusicStore.NetFramework.WebApp.ViewModels;
using NLog;

namespace MusicStore.NetFramework.WebApp.Controllers
{
	public class HomeController : Controller
	{
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		private readonly StoreContext _context;
		private readonly ICacheProvider _cache;

		public HomeController(StoreContext context, ICacheProvider cache)
		{
			_context = context;
			_cache = cache;
		}

		// GET: Home
		public ActionResult Index()
		{
			_logger.Info("Visited main page");

			List<Album> newArrivals;

			if (_cache.IsSet(Consts.NewItemsCacheKey))
			{
				newArrivals = _cache.Get(Consts.NewItemsCacheKey) as List<Album>;
			}
			else
			{
				newArrivals = _context.Albums
					.Where(x => x.IsHidden == false)
					.OrderByDescending(a => a.DateAdded)
					.Take(3)
					.ToList();

				_cache.Set(Consts.NewItemsCacheKey, newArrivals, 30);
			}

			// INFO - pobieranie danych z dazy w losowej kolejności - OrderBy(x => Guid.NewGuid())
			var bestsellers = _context.Albums
				.Where(x => x.IsHidden == false && x.IsBestseller)
				.OrderBy(x => Guid.NewGuid())
				.Take(3)
				.ToList();

			var vm = new HomeViewModel
			{
				Genres = _context.Genres.ToList(),
				NewArrivals = newArrivals,
				Bestsellers = bestsellers,
			};

			return View(vm);
		}

		public ActionResult StaticContent(string viewName) => View(viewName);
	}
}