using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace MusicStore.NetFramework.WebApp.Infrastructure
{
	public class Helpers
	{
		public static void CallUrl(string serviceUrl)
		{
			// Here we can't use Url.Action - called out of process
			var req = HttpWebRequest.Create(serviceUrl);
			req.GetResponseAsync();
		}
	}
}