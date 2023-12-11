using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MusicStore.NetFramework.WebApp.Controllers
{
	// INFO - bazowy kontroler dla pozostałych kontrolerów, można wtedy w łatwy sposób uzupełniać jakieś dane, z których pozostałe akcje będą korzystały
    public class BaseController : Controller
    {
        public BaseController()
        {
			// INFO - role użytkowników lepiej zapisać w jakimś enum niż stringi
			ViewBag.IsAdmin = User.IsInRole("admin");
        }
    }
}