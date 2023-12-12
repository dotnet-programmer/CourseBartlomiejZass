using System.Collections.Generic;
using MusicStore.NetFramework.WebApp.DAL;
using MusicStore.NetFramework.WebApp.Models;
using MvcSiteMapProvider;

namespace MusicStore.NetFramework.WebApp.Infrastructure
{
	// INFO - Mapa strony, używany w Mvc.sitemap
	public class ProductDetailsDynamicNodeProvider : DynamicNodeProviderBase
	{
		private readonly StoreContext _context = new StoreContext();

		public override IEnumerable<DynamicNode> GetDynamicNodeCollection(ISiteMapNode node)
		{
			// Build value
			var returnValue = new List<DynamicNode>();

			foreach (Album a in _context.Albums)
			{
				DynamicNode n = new DynamicNode
				{
					Title = a.AlbumTitle,
					Key = "Album_" + a.AlbumId,
					ParentKey = "Genre_" + a.GenreId
				};
				n.RouteValues.Add("id", a.AlbumId);
				returnValue.Add(n);
			}

			return returnValue;
		}
	}
}