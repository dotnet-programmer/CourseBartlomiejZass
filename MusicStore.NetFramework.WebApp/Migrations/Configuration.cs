using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using MusicStore.NetFramework.WebApp.DAL;

namespace MusicStore.NetFramework.WebApp.Migrations
{
	public sealed class Configuration : DbMigrationsConfiguration<MusicStore.NetFramework.WebApp.DAL.StoreContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "MusicStore.NetFramework.WebApp.DAL.StoreContext";
        }

        protected override void Seed(MusicStore.NetFramework.WebApp.DAL.StoreContext context)
        {
			//  This method will be called after migrating to the latest version.

			//  You can use the DbSet<T>.AddOrUpdate() helper extension method
			//  to avoid creating duplicate seed data.

			StoreInitializer.SeedStoreData(context);
			//StoreInitializer.InitializeIdentityForEF(context);
		}
    }
}
