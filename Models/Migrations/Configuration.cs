namespace Models.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            ContextKey = "Models.ApplicationDbContext";
        }

        protected override void Seed(Models.ApplicationDbContext context)
        {
            if (!context.Users.Any(u => u.PersonnelCode == "0"))
            {
                context.Users.Add(new User
                {
                    PersonnelCode = "0",
                    Password = "123456",
                    FirstName = "مدیر",
                    LastName = "سیستم",
                    IsConfirmed = true,
                    Role = SecurityRoles.Admin,
                    RegisterDate = DateTime.Now
                });
            }
        }
    }
}
