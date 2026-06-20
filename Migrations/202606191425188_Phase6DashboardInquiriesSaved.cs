namespace KarachiEstateHub.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Phase6DashboardInquiriesSaved : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Inquiries", "IsRead", c => c.Boolean(nullable: false));
            AddColumn("dbo.SavedProperties", "SavedAt", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SavedProperties", "SavedAt");
            DropColumn("dbo.Inquiries", "IsRead");
        }
    }
}
