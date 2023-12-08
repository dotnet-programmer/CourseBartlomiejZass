using System.Configuration;

namespace MusicStore.NetFramework.WebApp.Infrastructure
{
	public class AppConfig
	{
		private static string _genreIconsFolderRelative = ConfigurationManager.AppSettings["GenreIconsFolder"];
		public static string GenreIconsFolderRelative => _genreIconsFolderRelative;

		private static string _photosFolderRelative = ConfigurationManager.AppSettings["PhotosFolder"];
		public static string PhotosFolderRelative => _photosFolderRelative;
	}
}