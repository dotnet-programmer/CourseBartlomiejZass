namespace MusicStore.NetFramework.WebApp.Migrations
{
	using System.Data.Entity.Migrations;

	public partial class Update : DbMigration
	{
		public override void Up()
		{
			DropForeignKey("dbo.Orders", "ApplicationUser_Id", "dbo.AspNetUsers");
			DropIndex("dbo.Orders", new[] { "ApplicationUser_Id" });
			RenameColumn(table: "dbo.Orders", name: "ApplicationUser_Id", newName: "UserId");
			AlterColumn("dbo.Albums", "AlbumTitle", c => c.String(nullable: false));
			AlterColumn("dbo.Albums", "ArtistName", c => c.String(nullable: false));
			AlterColumn("dbo.Orders", "UserId", c => c.String(nullable: false, maxLength: 128));
			CreateIndex("dbo.Orders", "UserId");
			AddForeignKey("dbo.Orders", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
		}

		public override void Down()
		{
			DropForeignKey("dbo.Orders", "UserId", "dbo.AspNetUsers");
			DropIndex("dbo.Orders", new[] { "UserId" });
			AlterColumn("dbo.Orders", "UserId", c => c.String(maxLength: 128));
			AlterColumn("dbo.Albums", "ArtistName", c => c.String());
			AlterColumn("dbo.Albums", "AlbumTitle", c => c.String());
			RenameColumn(table: "dbo.Orders", name: "UserId", newName: "ApplicationUser_Id");
			CreateIndex("dbo.Orders", "ApplicationUser_Id");
			AddForeignKey("dbo.Orders", "ApplicationUser_Id", "dbo.AspNetUsers", "Id");
		}
	}
}