using System.Web.Mvc;

namespace MusicStore.NetFramework.WebApp.Controllers
{
	// INFO - bazowy kontroler dla pozostałych kontrolerów, można wtedy w łatwy sposób uzupełniać jakieś dane, z których pozostałe akcje będą korzystały
	public class BaseController : Controller
	{
		// INFO - role użytkowników lepiej zapisać w jakimś enum niż stringi
		public BaseController() => ViewBag.IsAdmin = User.IsInRole("admin");
	}
}