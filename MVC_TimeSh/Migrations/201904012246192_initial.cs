namespace MVC_TimeSh.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                        Birthday = c.DateTime(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);

            //CreateTable(
            //   "dbo.TimeSheetMaster",
            //   c => new
            //   {
            //       TimeMasterId = c.Int(nullable: false, identity: true),
            //       FromDate = c.DateTime(nullable: true),
            //       ToDate = c.DateTime(nullable: true),
            //       TotalHours = c.Int(nullable: true),
            //       UserId = c.Int(nullable: true),
            //       CreatedOn = c.DateTime(nullable: true),
            //       Comment = c.String(maxLength: 256),
            //       TimeSheetStatus = c.Int(nullable: true),
            //   })
            //   .PrimaryKey(t => t.TimeMasterId)
            //   .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
            //   .Index(t => t.UserId);

            //CreateTable(
            //    "dbo.TimeSheetDetails",
            //    c => new
            //    {
            //        TimeSheetId = c.Int(nullable: false, identity: true),
            //        DaysOfWeek = c.String(nullable: true, maxLength: 256),
            //        Hours = c.Int(nullable: true),
            //        DatePeriod = c.DateTime(nullable: true),
            //        ProjectId = c.Int(nullable: true),
            //        UserId = c.Int(nullable: true),
            //        DateCreated = c.DateTime(nullable: true),
            //        TimeMasterId = c.Int(nullable: true),
            //        TotalHours = c.Int(nullable: true),
            //    })
            //    .PrimaryKey(t => t.TimeSheetId)
            //    .ForeignKey("dbo.TimeSheetMaster", t => t.TimeMasterId, cascadeDelete: true)
            //    .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: false)
            //    .ForeignKey("dbo.ProjectMaster", t => t.ProjectId, cascadeDelete: true)
            //    .Index(t => t.UserId);

            //CreateTable(
            //    "dbo.TimeSheetAudit",
            //    c => new
            //    {
            //        ApprovalTimeSheetLogId = c.Int(nullable: false, identity: true),
            //        ApprovalUser = c.Int(nullable: true),
            //        DateProcessed = c.DateTime(nullable: true),
            //        DateCreated = c.DateTime(nullable: true),
            //        Comment = c.String(nullable: true, maxLength: 256),
            //        Status = c.Int(nullable: true),
            //        TimeSheetId = c.Int(nullable: true),
            //        UserId = c.Int(nullable: true),
            //    })
            //    .PrimaryKey(t => t.ApprovalTimeSheetLogId)
            //    .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
            //    .Index(t => t.UserId);

            //CreateTable(
            //    "dbo.ProjectMaster",
            //    c => new
            //    {
            //        ProjectId = c.Int(nullable: false, identity: true),
            //        ProjectName = c.String(nullable: true, maxLength: 128),
            //        NatureOfIndustry = c.String(nullable: true, maxLength: 128),
            //        ProjectCode = c.String(nullable: true, maxLength: 25),
            //    })
            //    .PrimaryKey(t => t.ProjectId)
            //    .Index(t => t.ProjectCode);

            //CreateTable(
            //    "dbo.Notifications",
            //    c => new
            //    {
            //        NotificationId = c.Int(nullable: false, identity: true),
            //        Status = c.String(nullable: true, maxLength: 50),
            //        Message = c.String(nullable: true, maxLength: 128),
            //        DateCreated = c.DateTime(nullable: true),
            //        FromDate = c.DateTime(nullable: true),
            //        ToDate = c.DateTime(nullable: true),
            //    })
            //    .PrimaryKey(t => t.NotificationId)
            //    .Index(t => t.Status);

            //CreateTable(
            //    "dbo.ExpenseAudit",
            //    c => new
            //    {
            //        ApprovalExpenseId = c.Int(nullable: false, identity: true),
            //        ApprovalUser = c.Int(nullable: true),
            //        DateProcessed = c.DateTime(nullable: true),
            //        DateCreated = c.DateTime(nullable: true),
            //        Comment = c.String(nullable: true, maxLength: 256),
            //        Status = c.Int(nullable: true),
            //        ExpenseId = c.Int(nullable: true),
            //        UserId = c.Int(nullable: true),
            //    })
            //    .PrimaryKey(t => t.ApprovalExpenseId)
            //    .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
            //    .Index(t => t.Status);

            //CreateTable(
            //    "dbo.Expense",
            //    c => new
            //    {
            //        ExpenseId = c.Int(nullable: false, identity: true),
            //        PurposeOrReason = c.String(nullable: true, maxLength: 128),
            //        ExpenseStatus = c.Int(nullable: true),
            //        FromDate = c.DateTime(nullable: true),
            //        ToDate = c.DateTime(nullable: true),
            //        VoucherId = c.Int(nullable: true),
            //        Bill1 = c.Decimal(nullable: true, scale: 2),
            //        Bill2 = c.Decimal(nullable: true, scale: 2),
            //        Bill3 = c.Decimal(nullable: true, scale: 2),
            //        TotalBill = c.Decimal(nullable: true, scale: 2),
            //        UserId = c.Int(nullable: true),
            //        DateCreated = c.DateTime(nullable: true),
            //        Comment = c.String(nullable: true, maxLength: 256),
            //        ProjectId = c.Int(nullable: true),
            //    })
            //    .PrimaryKey(t => t.ExpenseId)
            //    .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true);

            //CreateTable(
            //    "dbo.ErrorTable",
            //    c => new
            //    {
            //        ErrorId = c.Int(nullable: false, identity: true),
            //        Application = c.String(nullable: true, maxLength: 60),
            //        Type = c.String(nullable: true, maxLength: 60),
            //        Message = c.String(nullable: false, maxLength: 256),
            //        StatusCode = c.Int(nullable: false),
            //        UserId = c.Int(nullable: true),
            //        DateCreated = c.DateTime(nullable: true),
            //        Sequence = c.Int(nullable: false),
            //        AllXML = c.String(nullable: false),
            //    })
            //    .PrimaryKey(t => t.ErrorId);

            //CreateTable(
            //    "dbo.Documents",
            //    c => new
            //    {
            //        DocumentId = c.Int(nullable: false, identity: true),
            //        DocumentName = c.String(nullable: true, maxLength: 60),
            //        UserId = c.Int(nullable: true),
            //        DateCreated = c.DateTime(nullable: true),
            //        ExpenseId = c.Int(nullable: true),
            //        DocumentType = c.String(nullable: true, maxLength: 20),
            //    })
            //    .PrimaryKey(t => t.DocumentId);

            //CreateTable(
            //    "dbo.Description",
            //    c => new
            //    {
            //        DescriptionId = c.Int(nullable: false, identity: true),
            //        Description = c.String(nullable: true, maxLength: 128),
            //        UserId = c.Int(nullable: true),
            //        ProjectId = c.Int(nullable: true),
            //        DateCreated = c.DateTime(nullable: true),
            //        TimeSheetMasterId = c.Int(nullable: true),
            //    })
            //    .PrimaryKey(t => t.DescriptionId);

            //CreateTable(
            //    "dbo.Audit",
            //    c => new
            //    {
            //        AuditId = c.Int(nullable: false, identity: true),
            //        UserId = c.Int(nullable: true),
            //        SessionId = c.Int(nullable: true),
            //        IPaddress = c.String(nullable: true, maxLength: 60),
            //        PageAccessed = c.String(nullable: true, maxLength: 200),
            //        LoggedInAt = c.DateTime(nullable: true),
            //        LoggedOutAt = c.DateTime(nullable: true),
            //        LoginStatus = c.String(nullable: true, maxLength: 128),
            //        ControllerName = c.String(nullable: true, maxLength: 128),
            //        ActionName = c.String(nullable: true, maxLength: 128),
            //        UrlReferrer = c.String(nullable: true, maxLength: 128),
            //    })
            //    .PrimaryKey(t => t.AuditId);

        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");

            //DropForeignKey("dbo.Expense", "UserId", "dbo.AspNetUsers");
            //DropForeignKey("dbo.ExpenseAudit", "UserId", "dbo.AspNetUsers");
            //DropForeignKey("dbo.TimeSheetAudit", "UserId", "dbo.AspNetUsers");
            //DropForeignKey("dbo.TimeSheetDetails", "UserId", "dbo.AspNetUsers");
            //DropForeignKey("dbo.TimeSheetDetails", "ProjectId", "dbo.ProjectMaster");
            //DropForeignKey("dbo.TimeSheetDetails", "TimeMasterId", "dbo.TimeSheetMaster");
            //DropForeignKey("dbo.TimeSheetMaster", "UserId", "dbo.AspNetUsers");
            //DropIndex("dbo.ExpenseAudit", new[] { "Status" });
            //DropIndex("dbo.Notifications", new[] { "Status" });
            //DropIndex("dbo.ProjectMaster", new[] { "ProjecrCode" });
            //DropIndex("dbo.TimeSheetAudit", new[] { "UserId" });
            //DropIndex("dbo.TimeSheetDetails", new[] { "UserId" });
            //DropIndex("dbo.TimeSheetMaster", new[] { "UserId" });
            //DropTable("dbo.TimeSheetMaster");
            //DropTable("dbo.TimeSheetDetails");
            //DropTable("dbo.TimeSheetAudit");
            //DropTable("dbo.ProjectMaster");
            //DropTable("dbo.Notifications");
            //DropTable("dbo.ExpenseAudit");
            //DropTable("dbo.Expense");
            //DropTable("dbo.ErrorTable");
            //DropTable("dbo.Documents");
            //DropTable("dbo.Description");
            //DropTable("dbo.Audit");
        }
    }
}
