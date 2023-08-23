namespace Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RolePropertyAddedToUserClass : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "Role", c => c.String(maxLength: 30));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "Role");
        }
    }
}
