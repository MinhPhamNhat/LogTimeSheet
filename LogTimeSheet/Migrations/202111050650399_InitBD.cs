namespace LogTimeSheet.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitBD : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Log",
                c => new
                    {
                        LogId = c.Int(nullable: false, identity: true),
                        Note = c.String(nullable: false),
                        InitTime = c.DateTime(nullable: false),
                        Stdtime = c.Double(nullable: false),
                        DateLog = c.DateTime(nullable: false),
                        Overtime = c.Double(nullable: false),
                        IsApproved = c.Boolean(nullable: false),
                        DateApproved = c.DateTime(nullable: false),
                        Subtask_SubtaskId = c.Int(nullable: false),
                        User_UserId = c.String(nullable: false, maxLength: 128),
                        UserApproved_UserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.LogId)
                .ForeignKey("dbo.Subtask", t => t.Subtask_SubtaskId, cascadeDelete: true)
                .ForeignKey("dbo.User", t => t.User_UserId, cascadeDelete: true)
                .ForeignKey("dbo.User", t => t.UserApproved_UserId)
                .Index(t => t.Subtask_SubtaskId)
                .Index(t => t.User_UserId)
                .Index(t => t.UserApproved_UserId);
            
            CreateTable(
                "dbo.Subtask",
                c => new
                    {
                        SubtaskId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Project_ProjectId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SubtaskId)
                .ForeignKey("dbo.Project", t => t.Project_ProjectId, cascadeDelete: true)
                .Index(t => t.Project_ProjectId);
            
            CreateTable(
                "dbo.Project",
                c => new
                    {
                        ProjectId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        ProjectCode = c.String(nullable: false),
                        Type = c.Boolean(nullable: false),
                        InitTime = c.DateTime(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        Manager_UserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ProjectId)
                .ForeignKey("dbo.User", t => t.Manager_UserId)
                .Index(t => t.Manager_UserId);
            
            CreateTable(
                "dbo.ProjectUsers",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        ProjectId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.ProjectId })
                .ForeignKey("dbo.Project", t => t.ProjectId, cascadeDelete: true)
                .ForeignKey("dbo.User", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.ProjectId);
            
            CreateTable(
                "dbo.User",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        Role = c.Int(nullable: false),
                        Position = c.String(nullable: false),
                        Name = c.String(nullable: false),
                        Username = c.String(nullable: false),
                        Password = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Log", "UserApproved_UserId", "dbo.User");
            DropForeignKey("dbo.Log", "User_UserId", "dbo.User");
            DropForeignKey("dbo.Log", "Subtask_SubtaskId", "dbo.Subtask");
            DropForeignKey("dbo.Subtask", "Project_ProjectId", "dbo.Project");
            DropForeignKey("dbo.Project", "Manager_UserId", "dbo.User");
            DropForeignKey("dbo.ProjectUsers", "UserId", "dbo.User");
            DropForeignKey("dbo.ProjectUsers", "ProjectId", "dbo.Project");
            DropIndex("dbo.ProjectUsers", new[] { "ProjectId" });
            DropIndex("dbo.ProjectUsers", new[] { "UserId" });
            DropIndex("dbo.Project", new[] { "Manager_UserId" });
            DropIndex("dbo.Subtask", new[] { "Project_ProjectId" });
            DropIndex("dbo.Log", new[] { "UserApproved_UserId" });
            DropIndex("dbo.Log", new[] { "User_UserId" });
            DropIndex("dbo.Log", new[] { "Subtask_SubtaskId" });
            DropTable("dbo.User");
            DropTable("dbo.ProjectUsers");
            DropTable("dbo.Project");
            DropTable("dbo.Subtask");
            DropTable("dbo.Log");
        }
    }
}
