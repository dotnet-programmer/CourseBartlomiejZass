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
	public class ProductDetailsDynamicNodeProvider : DynamicNodeProviderBase
	{
		private StoreContext _db = new StoreContext();

		public override IEnumerable<DynamicNode> GetDynamicNodeCollection(ISiteMapNode node)
		{
			var returnValue = new List<DynamicNode>();

			foreach (Album a in _db.Albums)
			{
				DynamicNode n = new DynamicNode();
				n.Title = a.AlbumTitle;
				n.Key = "Album_" + a.AlbumId;
				n.ParentKey = "Genre_" + a.GenreId;
				n.RouteValues.Add("id", a.AlbumId);
				returnValue.Add(n);
			}

			return returnValue;
		}
	}
}