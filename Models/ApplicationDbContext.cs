using System.Data.Entity;

namespace Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() : base("Watchers")
        {
        }

        static ApplicationDbContext()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext, Migrations.Configuration>());
        }

        public DbSet<College> Colleges { get; set; }
        public DbSet<ExamPlace> ExamPlaces { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Watcher> Watchers { get; set; }
        public DbSet<WatcherExam> WatchersExams { get; set; }
        public DbSet<UserCollegeAccess> UserCollegeAccess { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
