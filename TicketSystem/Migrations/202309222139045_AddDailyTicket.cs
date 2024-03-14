namespace TicketSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDailyTicket : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DailyTickets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.Long(nullable: false),
                        Price = c.Int(nullable: false),
                        DateOfCreation = c.DateTime(nullable: false),
                        Group = c.Boolean(nullable: false),
                        NumOfPeople = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DailyTickets");
        }
    }
}
