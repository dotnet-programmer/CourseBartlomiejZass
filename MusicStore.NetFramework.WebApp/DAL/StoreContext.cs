using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using MusicStore.NetFramework.WebApp.Models;

namespace MusicStore.NetFramework.WebApp.DAL
{
	public class StoreContext : IdentityDbContext<ApplicationUser>
	{
		public StoreContext() : base("StoreContext")
		{
		}

		// INFO - dodanie inicjalizatorów bazy danych - sposób 3 - najczęściej używany
		static StoreContext() => Database.SetInitializer<StoreContext>(new StoreInitializer());

		public virtual DbSet<Album> Albums { get; set; }
		public DbSet<Genre> Genres { get; set; }
		public virtual DbSet<Order> Orders { get; set; }
		public DbSet<OrderItem> OrderItems { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// modelBuilder.Entity<ApplicationUser>().HasMany(a => a.Orders).WithRequired().WillCascadeOnDelete(true);
			modelBuilder.Entity<Order>().HasRequired(o => o.User);

			//.HasForeignKey(p => p.DepartmentId)
			//.WillCascadeOnDelete(false);

			// Change the name of the table to be Users instead of AspNetUsers
			//modelBuilder.Entity<IdentityUser>()
			//    .ToTable("Users");
			//modelBuilder.Entity<ApplicationUser>()
			//    .ToTable("Users");
		}

		public static StoreContext Create() => new StoreContext();
	}
}