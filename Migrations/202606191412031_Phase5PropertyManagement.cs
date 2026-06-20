namespace KarachiEstateHub.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Phase5PropertyManagement : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Properties", "PriceType", c => c.String(maxLength: 30));
            AddColumn("dbo.Properties", "StreetAddress", c => c.String(maxLength: 300));
            AddColumn("dbo.Properties", "ContactNumber", c => c.String(maxLength: 30));
            AddColumn("dbo.Properties", "WhatsAppNumber", c => c.String(maxLength: 30));
            AddColumn("dbo.Properties", "ContactEmail", c => c.String(maxLength: 150));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Properties", "ContactEmail");
            DropColumn("dbo.Properties", "WhatsAppNumber");
            DropColumn("dbo.Properties", "ContactNumber");
            DropColumn("dbo.Properties", "StreetAddress");
            DropColumn("dbo.Properties", "PriceType");
        }
    }
}
