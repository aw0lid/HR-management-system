using Domain.Entites;
using Infrastructure.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class HRDBContext : DbContext
    {
        public HRDBContext(DbContextOptions<HRDBContext> options) : base(options)
        {
           
        }

        public HRDBContext(){}

        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<EmployeeWorkInfo> EmployeeWorkInfos => Set<EmployeeWorkInfo>();
        public DbSet<Nationality> Nationalities => Set<Nationality>();
        public DbSet<EmployeePhone> EmployeePhones => Set<EmployeePhone>();
        public DbSet<EmployeeEmail> EmployeeEmails => Set<EmployeeEmail>();
        public DbSet<Department> Departments => Set<Department>();
        public DbSet<JobGrade> JobGrades => Set<JobGrade>();
        public DbSet<JobTitle> JobTitles => Set<JobTitle>();
        public DbSet<JobTitleLevel> JobTitleLevels => Set<JobTitleLevel>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Token> Tokens => Set<Token>();
        public DbSet<UserLog> UserLogs => Set<UserLog>();
        public DbSet<PendingAdminAction> PendingAdminActions => Set<PendingAdminAction>();


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=localhost,1433;Database=HR_System_DB;User Id=sa;Password=Ahmed@123456;TrustServerCertificate=True;MultipleActiveResultSets=true")
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors();
            }
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(HRDBContext).Assembly);
            DataSeedProvider.Seed(modelBuilder);
        }
    }
}