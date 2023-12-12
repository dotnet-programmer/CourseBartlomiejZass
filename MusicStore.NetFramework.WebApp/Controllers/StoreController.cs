using System.Linq;
using System.Web.Mvc;
using MusicStore.NetFramework.WebApp.DAL;

namespace MusicStore.NetFramework.WebApp.Controllers
{
	public class StoreController : Controller
	{
		private readonly StoreContext _context = new StoreContext();

		// GET: Store
		public ActionResult Index() => View();

		public ActionResult Details(int id) => View(_context.Albums.Find(id));

		public ActionResult List(string genreName, string searchQuery = null)
		{
			var genre = _context.Genres
				.Include("Albums")
				.Where(x => x.Name.ToLower() == genreName.ToLower())
				.Single();

			var albums = genre.Albums
				.Where(a => !a.IsHidden && (searchQuery == null
				 || a.AlbumTitle.ToLower().Contains(searchQuery.ToLower())
				 || a.ArtistName.ToLower().Contains(searchQuery.ToLower())));

			if (Request.IsAjaxRequest())
			{
				return PartialView("_ProductList", albums);
			}
			return View(albums);
		}

		[ChildActionOnly]
		[OutputCache(Duration = 86400)]
		public ActionResult GenresMenu() => PartialView("_GenresMenu", _context.Genres.ToList());

		// INFO - widżet autocomplete z jquery.ui wysyła parametr "term" który zawiera dane z formularza
		public ActionResult AlbumsSuggestions(string term)
		{
			var albums = _context.Albums
				.Where(x => !x.IsHidden && x.AlbumTitle.ToLower().Contains(term.ToLower()))
				.Take(5)
				.Select(a => new { label = a.AlbumTitle });

			return Json(albums, JsonRequestBehavior.AllowGet);
		}

		protected override void Dispose(bool disposing)
		{
			_context?.Dispose();
			base.Dispose(disposing);
		}
	}
}