namespace Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserCollegeAccessImplemented : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserCollegeAccesses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserCode = c.String(maxLength: 15),
                        CollegeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Colleges", t => t.CollegeId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserCode)
                .Index(t => t.UserCode)
                .Index(t => t.CollegeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserCollegeAccesses", "UserCode", "dbo.Users");
            DropForeignKey("dbo.UserCollegeAccesses", "CollegeId", "dbo.Colleges");
            DropIndex("dbo.UserCollegeAccesses", new[] { "CollegeId" });
            DropIndex("dbo.UserCollegeAccesses", new[] { "UserCode" });
            DropTable("dbo.UserCollegeAccesses");
        }
    }
}
