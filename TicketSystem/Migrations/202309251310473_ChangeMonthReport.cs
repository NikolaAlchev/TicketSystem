namespace TicketSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeMonthReport : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.MonthlyReports", "Month", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.MonthlyReports", "Month", c => c.String());
        }
    }
}
