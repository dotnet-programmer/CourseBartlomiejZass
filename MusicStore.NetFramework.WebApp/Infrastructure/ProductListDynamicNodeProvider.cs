using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MusicStore.NetFramework.WebApp.DAL;
using MusicStore.NetFramework.WebApp.Models;
using MvcSiteMapProvider;

namespace MusicStore.NetFramework.WebApp.Infrastructure
{
	// INFO - Mapa strony, używany w Mvc.sitemap
	public class ProductListDynamicNodeProvider : DynamicNodeProviderBase
	{
		private StoreContext _db = new StoreContext();

		public override IEnumerable<DynamicNode> GetDynamicNodeCollection(ISiteMapNode node)
		{
			var returnValue = new List<DynamicNode>();

			foreach (Genre g in _db.Genres)
			{
				DynamicNode n = new DynamicNode();
				n.Title = g.Name;
				n.Key = "Genre_" + g.GenreId;
				n.RouteValues.Add("genreName", g.Name);
				returnValue.Add(n);
			}

			return returnValue;
		}
	}
}