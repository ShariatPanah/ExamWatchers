namespace Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovedRequiredAttributeFromPlaceDescription : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ExamPlaces", "Description", c => c.String(maxLength: 200));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ExamPlaces", "Description", c => c.String(nullable: false, maxLength: 200));
        }
    }
}
