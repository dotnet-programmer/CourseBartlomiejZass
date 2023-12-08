using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using MusicStore.NetFramework.WebApp.Models;

namespace MusicStore.NetFramework.WebApp.DAL
{
	public class StoreContext : DbContext
	{
		public StoreContext() : base("StoreContext")
		{

		}

		// dodanie inicjalizatorów bazy danych - sposób 3 - najczęściej używany
		static StoreContext() => Database.SetInitializer<StoreContext>(new StoreInitializer());

		public DbSet<Album> Albums { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
    }
}