namespace TicketSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeToLong : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Tickets", "Code", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Tickets", "Code", c => c.Int(nullable: false));
        }
    }
}
