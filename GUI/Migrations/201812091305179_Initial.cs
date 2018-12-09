namespace GUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SMGameAndCards",
                c => new
                    {
                        Key = c.Int(nullable: false, identity: true),
                        CardsCount = c.Int(nullable: false),
                        CardsAveragePrice = c.Double(nullable: false),
                        ChanceToPayOff = c.Double(nullable: false),
                        Game_Key = c.Int(),
                    })
                .PrimaryKey(t => t.Key)
                .ForeignKey("dbo.SSGames", t => t.Game_Key)
                .Index(t => t.Game_Key);
            
            CreateTable(
                "dbo.SSGames",
                c => new
                    {
                        Key = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Price = c.Double(nullable: false),
                        Link = c.String(),
                        ImageUrl = c.String(),
                    })
                .PrimaryKey(t => t.Key);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SMGameAndCards", "Game_Key", "dbo.SSGames");
            DropIndex("dbo.SMGameAndCards", new[] { "Game_Key" });
            DropTable("dbo.SSGames");
            DropTable("dbo.SMGameAndCards");
        }
    }
}
