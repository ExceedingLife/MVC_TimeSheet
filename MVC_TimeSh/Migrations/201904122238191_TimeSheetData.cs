namespace MVC_TimeSh.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TimeSheetData : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TimeSheetDetails",
                c => new
                    {
                        TimeSheetId = c.Int(nullable: false, identity: true),
                        DaysOfWeek = c.String(),
                        Hours = c.Int(),
                        Period = c.DateTime(),
                        ProjectId = c.Int(),
                        UserId = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        TimeSheetMasterId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TimeSheetId);                
            
            CreateTable(
                "dbo.TimeSheetMasters",
                c => new
                    {
                        TimeSheetMasterId = c.Int(nullable: false, identity: true),
                        FromDate = c.DateTime(nullable: true),
                        ToDate = c.DateTime(nullable: true),
                        TotalHours = c.Int(nullable: true),
                        UserId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: true),
                        TimeSheetStatus = c.Int(nullable: true),
                        Comment = c.String(maxLength: 256),
                    })
                .PrimaryKey(t => t.TimeSheetMasterId);
                
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TimeSheetMasters");
            DropTable("dbo.TimeSheetDetails");
        }
    }
}
