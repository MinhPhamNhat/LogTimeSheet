namespace LogTimeSheet.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Log", "DateApproved", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Log", "DateApproved", c => c.DateTime(nullable: false));
        }
    }
}
