namespace Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OrganFieldAddedToUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "Organ", c => c.Byte(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "Organ");
        }
    }
}
