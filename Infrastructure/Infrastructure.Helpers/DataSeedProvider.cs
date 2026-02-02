using Domain.Entites;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Helpers
{
    public static class DataSeedProvider
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
    
            modelBuilder.Entity<Nationality>().HasData(
                new { NationalityId = 1, NationalityName = "Egyptian" },
                new { NationalityId = 2, NationalityName = "Saudi" },
                new { NationalityId = 3, NationalityName = "Emirati" },
                new { NationalityId = 4, NationalityName = "Jordanian" },
                new { NationalityId = 5, NationalityName = "American" }
            );

            
            modelBuilder.Entity<Department>().HasData(
                new { DepartmentId = 1, DepartmentName = "Information Technology", DepartmentCode = "IT", DepartmentDescription = "Infrastructure & Software Development" },
                new { DepartmentId = 2, DepartmentName = "Human Resources", DepartmentCode = "HR", DepartmentDescription = "Talent Management & Employee Relations" },
                new { DepartmentId = 3, DepartmentName = "Finance & Accounting", DepartmentCode = "FIN", DepartmentDescription = "Financial Planning & Payroll" },
                new { DepartmentId = 4, DepartmentName = "Sales & Marketing", DepartmentCode = "S&M", DepartmentDescription = "Revenue Generation & Brand Awareness" },
                new { DepartmentId = 5, DepartmentName = "Operations", DepartmentCode = "OPS", DepartmentDescription = "Core Business Operations & Logistics" }
            );

           
            modelBuilder.Entity<JobTitle>().HasData(
                new { JobTitleId = 1, JobTitleName = "Backend .NET Developer", JobTitleCode = "BEDEV", JobTitleDescription = "Developing scalable backend systems" },
                new { JobTitleId = 2, JobTitleName = "HR Generalist", JobTitleCode = "HRGEN", JobTitleDescription = "Handling day-to-day HR operations" },
                new { JobTitleId = 3, JobTitleName = "Financial Accountant", JobTitleCode = "ACC", JobTitleDescription = "Managing company accounts and taxes" },
                new { JobTitleId = 4, JobTitleName = "System Administrator", JobTitleCode = "SYSAD", JobTitleDescription = "Managing server infrastructure" },
                new { JobTitleId = 5, JobTitleName = "Operations Manager", JobTitleCode = "OPSM", JobTitleDescription = "Overseeing operational workflows" }
            );


            modelBuilder.Entity<JobGrade>().HasData(
               new { JobGradeId = 1, JobGradeName = "Junior Level", GradeCode = "G1", LevelDescription = "Entry level for fresh graduates", Weight = 100 },
               new { JobGradeId = 2, JobGradeName = "Mid Level", GradeCode = "G2", LevelDescription = "Individual contributor with experience", Weight = 250 },
               new { JobGradeId = 3, JobGradeName = "Senior Level", GradeCode = "G3", LevelDescription = "Subject matter expert", Weight = 450 },
               new { JobGradeId = 4, JobGradeName = "Team Lead", GradeCode = "G4", LevelDescription = "Technical or functional leadership", Weight = 700 },
               new { JobGradeId = 5, JobGradeName = "Management", GradeCode = "G5", LevelDescription = "Strategic leadership and decision making", Weight = 1000 }
);


            modelBuilder.Entity<JobTitleLevel>().HasData(
              
                new { JobTitleLevelId = 1, JobTitleId = 1, JobGradeId = 1 }, 
                new { JobTitleLevelId = 2, JobTitleId = 1, JobGradeId = 2 },
                new { JobTitleLevelId = 3, JobTitleId = 1, JobGradeId = 3 },
                new { JobTitleLevelId = 4, JobTitleId = 1, JobGradeId = 4 },

               
                new { JobTitleLevelId = 5, JobTitleId = 2, JobGradeId = 1 },
                new { JobTitleLevelId = 6, JobTitleId = 2, JobGradeId = 2 },

               
                new { JobTitleLevelId = 7, JobTitleId = 5, JobGradeId = 4 },
                new { JobTitleLevelId = 8, JobTitleId = 5, JobGradeId = 5 }
            );
        }
    }
}