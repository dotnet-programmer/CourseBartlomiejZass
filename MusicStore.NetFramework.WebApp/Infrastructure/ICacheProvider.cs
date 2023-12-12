namespace MusicStore.NetFramework.WebApp.Infrastructure
{
	public interface ICacheProvider
	{
		object Get(string key);
		void Set(string key, object data, int cacheTime);
		bool IsSet(string key);
		void Invalidate(string key);
	}
}