using System.Web.Mvc;
using System.Web.Routing;

namespace MusicStore.NetFramework.WebApp
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
				name: "ProductDetails",
				url: "album-{id}.html",
				defaults: new { controller = "Store", action = "Details" }
			);

			routes.MapRoute(
				name: "StaticPages",
				url: "strony/{viewName}.html",
				defaults: new { controller = "Home", action = "StaticContent" }
			);

			routes.MapRoute(
				name: "ProductList",
				url: "gatunki/{genreName}",
				defaults: new { controller = "Store", action = "List" },

				// INFO - ograniczenie w routingu - parametr genreName może zawierać tylko znaki alfanumeryczne, znak "&" oraz spację
				constraints: new { genreName = @"[\w& ]+" }
			);

			routes.MapRoute(
				name: "Default",
				url: "{controller}/{action}/{id}",
				defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
			);
		}
	}
}