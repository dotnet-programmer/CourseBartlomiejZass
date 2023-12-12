using System.IO;
using System.Web.Mvc;

namespace MusicStore.NetFramework.WebApp.Infrastructure
{
	public static class UrlHelpers
	{
		public static string GenreIconPath(this UrlHelper helper, string genreIconFilename)
			=> helper.Content(Path.Combine(AppConfig.GenreIconsFolderRelative, genreIconFilename));

		public static string AlbumCoverPath(this UrlHelper helper, string albumFilename)
			=> helper.Content(Path.Combine(AppConfig.PhotosFolderRelative, albumFilename));
	}
}