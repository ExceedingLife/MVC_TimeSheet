namespace MVC_TimeSh.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Projects : DbMigration
    {//ProjectMaster
        public override void Up()
        {
            CreateTable(
                "dbo.Project",
                c => new
                {
                    ProjectId = c.Int(nullable: false, identity: true),
                    ProjectName = c.String(nullable: true, maxLength: 128),
                    NatureOfIndustry = c.String(nullable: true, maxLength: 128),
                    ProjectCode = c.String(nullable: true, maxLength: 25),
                })
                .PrimaryKey(t => t.ProjectId)
                .Index(t => t.ProjectCode);
        }
        
        public override void Down()
        {
            DropIndex("dbo.ProjectMaster", new[] { "ProjectCode" });
            DropTable("dbo.ProjectMaster");
        }
    }
}
