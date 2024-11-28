namespace MusicStore.NetFramework.WebApp.Migrations
{
	using System.Data.Entity.Migrations;

	public partial class InitialCreate : DbMigration
	{
		public override void Up()
		{
			CreateTable(
				"dbo.Albums",
				c => new
				{
					AlbumId = c.Int(nullable: false, identity: true),
					AlbumTitle = c.String(),
					ArtistName = c.String(),
					DateAdded = c.DateTime(nullable: false),
					CoverFileName = c.String(),
					Description = c.String(),
					Price = c.Decimal(nullable: false, precision: 18, scale: 2),
					IsBestseller = c.Boolean(nullable: false),
					IsHidden = c.Boolean(nullable: false),
					GenreId = c.Int(nullable: false),
				})
				.PrimaryKey(t => t.AlbumId)
				.ForeignKey("dbo.Genres", t => t.GenreId, cascadeDelete: true)
				.Index(t => t.GenreId);

			CreateTable(
				"dbo.Genres",
				c => new
				{
					GenreId = c.Int(nullable: false, identity: true),
					Name = c.String(),
					Description = c.String(),
					IconFileName = c.String(),
				})
				.PrimaryKey(t => t.GenreId);

			CreateTable(
				"dbo.OrderItems",
				c => new
				{
					OrderItemId = c.Int(nullable: false, identity: true),
					Quantity = c.Int(nullable: false),
					UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
					AlbumId = c.Int(nullable: false),
					OrderId = c.Int(nullable: false),
				})
				.PrimaryKey(t => t.OrderItemId)
				.ForeignKey("dbo.Albums", t => t.AlbumId, cascadeDelete: true)
				.ForeignKey("dbo.Orders", t => t.OrderId, cascadeDelete: true)
				.Index(t => t.AlbumId)
				.Index(t => t.OrderId);

			CreateTable(
				"dbo.Orders",
				c => new
				{
					OrderId = c.Int(nullable: false, identity: true),
					FirstName = c.String(nullable: false, maxLength: 100),
					LastName = c.String(nullable: false, maxLength: 150),
					Address = c.String(nullable: false, maxLength: 150),
					CodeAndCity = c.String(nullable: false, maxLength: 50),
					PhoneNumber = c.String(nullable: false, maxLength: 20),
					Email = c.String(nullable: false),
					Comment = c.String(),
					DateCreated = c.DateTime(nullable: false),
					OrderState = c.Int(nullable: false),
					TotalPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
				})
				.PrimaryKey(t => t.OrderId);
		}

		public override void Down()
		{
			DropForeignKey("dbo.OrderItems", "OrderId", "dbo.Orders");
			DropForeignKey("dbo.OrderItems", "AlbumId", "dbo.Albums");
			DropForeignKey("dbo.Albums", "GenreId", "dbo.Genres");
			DropIndex("dbo.OrderItems", new[] { "OrderId" });
			DropIndex("dbo.OrderItems", new[] { "AlbumId" });
			DropIndex("dbo.Albums", new[] { "GenreId" });
			DropTable("dbo.Orders");
			DropTable("dbo.OrderItems");
			DropTable("dbo.Genres");
			DropTable("dbo.Albums");
		}
	}
}