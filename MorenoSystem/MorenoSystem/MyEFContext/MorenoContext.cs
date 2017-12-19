using System.Data.Entity;
using MorenoSystem.Entities;
using MorenoSystem.MyEFContext.Initializers;
using MySql.Data.Entity;

namespace MorenoSystem.MyEFContext
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]

    public class MorenoContext : DbContext
    {
        public MorenoContext() : base("name=AbmuConnString")
        {
            //Database.Exists();
            Database.SetInitializer(new NotExistInitializer());
            Database.SetInitializer(new ModelChangeInitializer());
        }
        public DbSet<Student> Students { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<CouncilPosition> CouncilPositions { get; set; }
        public DbSet<YearLevel> YearLevels { get; set; }
        public DbSet<Requirement> Requirements { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<CouncilMembers> CouncilMembers { get; set; }
        public DbSet<PartyList> PartyLists { get; set; }
        public DbSet<StudentVote> StudentVotes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<RequirementStudents> RequirementStudents { get; set; }
        public DbSet<Category> Categories { get; set; }public DbSet<TeacherBorrowedBook> TeacherBorrowedBooks { get; set; }
        public DbSet<Source> Sources { get; set; }
        public DbSet<Condition> Conditions { get; set; }
        public DbSet<BookSource> BookSources { get; set; }
        public DbSet<ElectionStatus> ElectionStatus { get; set; }
        public DbSet<ElectionHistory> ElectionHistory { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>()
                .HasOptional(c => c.User)
                .WithOptionalDependent(c => c.Student)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Teacher>()
                .HasOptional(c => c.User)
                .WithOptionalDependent(c => c.Teacher)
                .WillCascadeOnDelete(true);
        }
    }
}