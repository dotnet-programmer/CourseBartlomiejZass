using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MusicStore.NetFramework.WebApp.DAL;

namespace MusicStore.NetFramework.WebApp.Controllers
{
	public class StoreController : Controller
	{
		private StoreContext _storeContext = new StoreContext();

		// GET: Store
		public ActionResult Index()
		{
			return View();
		}

		public ActionResult Details(int id)
		{
			return View();
		}

		public ActionResult List(string genreName)
		{
			var genre = _storeContext.Genres
				.Include("Albums")
				.Where(x=>x.Name.ToLower() == genreName.ToLower())
				.Single();

			var albums = genre.Albums.ToList();

			return View(albums);
		}

		[ChildActionOnly]
		[OutputCache(Duration = 86400)]
		public ActionResult GenresMenu()
		{
			return PartialView("_GenresMenu", _storeContext.Genres.ToList());
		}
	}
}