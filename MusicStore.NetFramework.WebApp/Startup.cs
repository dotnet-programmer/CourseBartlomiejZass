﻿using Owin;

namespace MusicStore.NetFramework.WebApp
{
	public partial class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			ConfigureAuth(app);

			//app.UseHangfire(config =>
			//{
			//	config.UseAuthorizationFilters(new AuthorizationFilter
			//	{
			//		Roles = "Admin"
			//	});

			//	config.UseSqlServerStorage("StoreContext");
			//	config.UseServer();
			//});
		}
	}
}