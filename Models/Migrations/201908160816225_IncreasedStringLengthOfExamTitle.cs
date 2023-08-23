namespace Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IncreasedStringLengthOfExamTitle : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Exams", "Title", c => c.String(nullable: false, maxLength: 40));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Exams", "Title", c => c.String(nullable: false, maxLength: 15));
        }
    }
}
