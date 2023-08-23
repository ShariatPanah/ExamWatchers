namespace Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemainingCapacityPropertyRemovedFromExam : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Exams", "RemainingCapacity");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Exams", "RemainingCapacity", c => c.Byte(nullable: false));
        }
    }
}
