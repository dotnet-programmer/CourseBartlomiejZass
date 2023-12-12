using System;
using System.Web;
using System.Web.Caching;

namespace MusicStore.NetFramework.WebApp.Infrastructure
{
	public class DefaultCacheProvider : ICacheProvider
	{
		private Cache Cache => HttpContext.Current.Cache;

		public object Get(string key) => Cache[key];

		public void Set(string key, object data, int cacheTimeMinutes)
		{
			var expirationTime = DateTime.Now + TimeSpan.FromMinutes(cacheTimeMinutes);
			Cache.Insert(key, data, null, expirationTime, Cache.NoSlidingExpiration);
		}

		public bool IsSet(string key) => (Cache[key] != null);

		public void Invalidate(string key) => Cache.Remove(key);
	}
}