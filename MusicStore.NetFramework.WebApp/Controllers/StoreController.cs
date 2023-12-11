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
			return View(_storeContext.Albums.Find(id));
		}

		public ActionResult List(string genreName, string searchQuery = null)
		{
			var genre = _storeContext.Genres
				.Include("Albums")
				.Where(x=>x.Name.ToLower() == genreName.ToLower())
				.Single();

			var albums = genre.Albums.Where(a => (searchQuery == null ||
												a.AlbumTitle.ToLower().Contains(searchQuery.ToLower()) ||
												a.ArtistName.ToLower().Contains(searchQuery.ToLower())) &&
												!a.IsHidden);

			if (Request.IsAjaxRequest())
			{
				return PartialView("_ProductList", albums);
			}
			return View(albums);
		}

		[ChildActionOnly]
		[OutputCache(Duration = 86400)]
		public ActionResult GenresMenu()
		{
			return PartialView("_GenresMenu", _storeContext.Genres.ToList());
		}

		// widżet autocomplete z jquery.ui wysyła parametr "term" który zawiera dane z formularza
		public ActionResult AlbumsSuggestions(string term)
		{
			var albums = this._storeContext.Albums
				.Where(x => !x.IsHidden && x.AlbumTitle.ToLower().Contains(term.ToLower()))
				.Take(5)
				.Select(a => new { label = a.AlbumTitle });

			return Json(albums, JsonRequestBehavior.AllowGet);
		}
	}
}