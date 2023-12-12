using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using MusicStore.NetFramework.WebApp.App_Start;

namespace MusicStore.NetFramework.WebApp
{
	public class MvcApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);

			// INFO - dodanie inicjalizatorów bazy danych - sposób 1
			// Database.SetInitializer<StoreContext>(new StoreInitializer());
		}
	}
}