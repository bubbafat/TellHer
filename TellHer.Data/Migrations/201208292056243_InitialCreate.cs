namespace TellHer.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Subscriptions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Phone = c.String(),
                        Next = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DailyIdeas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Idea = c.String(),
                        LastUsed = c.DateTime(nullable: false),
                        Theme_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.WeeklyThemes", t => t.Theme_Id)
                .Index(t => t.Theme_Id);
            
            CreateTable(
                "dbo.WeeklyThemes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Theme = c.String(),
                        LastUsed = c.DateTime(nullable: false),
                        Scheduled = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.DailyIdeas", new[] { "Theme_Id" });
            DropForeignKey("dbo.DailyIdeas", "Theme_Id", "dbo.WeeklyThemes");
            DropTable("dbo.WeeklyThemes");
            DropTable("dbo.DailyIdeas");
            DropTable("dbo.Subscriptions");
        }
    }
}
