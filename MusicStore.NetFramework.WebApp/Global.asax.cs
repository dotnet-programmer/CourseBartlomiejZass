using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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

			// dodanie inicjalizatorów bazy danych - sposób 1
			// Database.SetInitializer<StoreContext>(new StoreInitializer());            
		}
	}
}
