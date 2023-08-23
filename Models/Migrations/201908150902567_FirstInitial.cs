namespace Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FirstInitial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Colleges",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 30),
                        ModifiedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ExamPlaces",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 80),
                        Description = c.String(nullable: false, maxLength: 200),
                        ModifiedDate = c.DateTime(nullable: false),
                        CollegeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Colleges", t => t.CollegeId, cascadeDelete: true)
                .Index(t => t.CollegeId);
            
            CreateTable(
                "dbo.Exams",
                c => new
                    {
                        Code = c.String(nullable: false, maxLength: 15),
                        Title = c.String(nullable: false, maxLength: 15),
                        ExamStartTime = c.DateTime(nullable: false),
                        ExamEndTime = c.DateTime(nullable: false),
                        WatchersCapacity = c.Byte(nullable: false),
                        RemainingCapacity = c.Byte(nullable: false),
                        ExamPlaceId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Code)
                .ForeignKey("dbo.ExamPlaces", t => t.ExamPlaceId, cascadeDelete: true)
                .Index(t => t.ExamPlaceId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        PersonnelCode = c.String(nullable: false, maxLength: 15),
                        Password = c.String(nullable: false, maxLength: 20),
                        FirstName = c.String(nullable: false, maxLength: 25),
                        LastName = c.String(nullable: false, maxLength: 40),
                        IsConfirmed = c.Boolean(nullable: false),
                        RegisterDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.PersonnelCode);
            
            CreateTable(
                "dbo.Watchers",
                c => new
                    {
                        PersonnelCode = c.String(nullable: false, maxLength: 15),
                        WatchHours = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.PersonnelCode)
                .ForeignKey("dbo.Users", t => t.PersonnelCode)
                .Index(t => t.PersonnelCode);
            
            CreateTable(
                "dbo.WatcherExams",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ExamCode = c.String(maxLength: 15),
                        WatcherCode = c.String(maxLength: 15),
                        SubmitDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Exams", t => t.ExamCode)
                .ForeignKey("dbo.Watchers", t => t.WatcherCode)
                .Index(t => t.ExamCode)
                .Index(t => t.WatcherCode);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WatcherExams", "WatcherCode", "dbo.Watchers");
            DropForeignKey("dbo.WatcherExams", "ExamCode", "dbo.Exams");
            DropForeignKey("dbo.Watchers", "PersonnelCode", "dbo.Users");
            DropForeignKey("dbo.Exams", "ExamPlaceId", "dbo.ExamPlaces");
            DropForeignKey("dbo.ExamPlaces", "CollegeId", "dbo.Colleges");
            DropIndex("dbo.WatcherExams", new[] { "WatcherCode" });
            DropIndex("dbo.WatcherExams", new[] { "ExamCode" });
            DropIndex("dbo.Watchers", new[] { "PersonnelCode" });
            DropIndex("dbo.Exams", new[] { "ExamPlaceId" });
            DropIndex("dbo.ExamPlaces", new[] { "CollegeId" });
            DropTable("dbo.WatcherExams");
            DropTable("dbo.Watchers");
            DropTable("dbo.Users");
            DropTable("dbo.Exams");
            DropTable("dbo.ExamPlaces");
            DropTable("dbo.Colleges");
        }
    }
}
