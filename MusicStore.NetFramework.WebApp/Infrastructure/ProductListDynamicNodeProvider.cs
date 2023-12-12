using System.Collections.Generic;
using MusicStore.NetFramework.WebApp.DAL;
using MusicStore.NetFramework.WebApp.Models;
using MvcSiteMapProvider;

namespace MusicStore.NetFramework.WebApp.Infrastructure
{
	// INFO - Mapa strony, używany w Mvc.sitemap
	public class ProductListDynamicNodeProvider : DynamicNodeProviderBase
	{
		private readonly StoreContext _context = new StoreContext();

		public override IEnumerable<DynamicNode> GetDynamicNodeCollection(ISiteMapNode node)
		{
			var returnValue = new List<DynamicNode>();

			foreach (Genre g in _context.Genres)
			{
				DynamicNode n = new DynamicNode
				{
					Title = g.Name,
					Key = "Genre_" + g.GenreId
				};
				n.RouteValues.Add("genreName", g.Name);
				returnValue.Add(n);
			}

			return returnValue;
		}
	}
}