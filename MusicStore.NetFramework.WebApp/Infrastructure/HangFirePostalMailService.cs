using System.Web;
using System.Web.Mvc;
using Hangfire;

namespace MusicStore.NetFramework.WebApp.Infrastructure
{
	public class HangFirePostalMailService : IMailService
	{
		public void SendOrderConfirmationEmail(Models.Order order)
		{
			var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
			string url = urlHelper.Action(
				"SendConfirmationEmail",
				"Manage",
				new { orderId = order.OrderId, lastName = order.LastName },
				HttpContext.Current.Request.Url.Scheme);

			// Hangfire - nice one (if ASP.NET app will be still running)
			BackgroundJob.Enqueue(() => Helpers.CallUrl(url));
		}

		public void SendOrderShippedEmail(Models.Order order)
		{
			var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
			string url = urlHelper.Action(
				"SendStatusEmail",
				"Manage",
				new { orderId = order.OrderId, lastName = order.LastName },
				HttpContext.Current.Request.Url.Scheme);

			BackgroundJob.Enqueue(() => Helpers.CallUrl(url));
		}
	}
}