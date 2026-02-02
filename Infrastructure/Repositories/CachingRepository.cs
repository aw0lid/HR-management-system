using Domain.Entites;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class DepartmentLoader(HRDBContext context) : IDataLoader<Department>
    {
        public async Task<List<Department>> GetAsync() => 
            await context.Departments.AsNoTracking().ToListAsync();
    }

    public class NationalityLoader(HRDBContext context) : IDataLoader<Nationality>
    {
        public async Task<List<Nationality>> GetAsync() => 
            await context.Nationalities.AsNoTracking().ToListAsync();
    }

    public class JobTitleLoader(HRDBContext context) : IDataLoader<JobTitle>
    {
        public async Task<List<JobTitle>> GetAsync() => 
            await context.JobTitles.AsNoTracking().ToListAsync();
    }

    public class JobGradeLoader(HRDBContext context) : IDataLoader<JobGrade>
    {
        public async Task<List<JobGrade>> GetAsync() => 
            await context.JobGrades.AsNoTracking().ToListAsync();
    }

    public class JobTitleLevelLoader(HRDBContext context) : IDataLoader<JobTitleLevel>
    {
        public async Task<List<JobTitleLevel>> GetAsync() => 
            await context.JobTitleLevels
                         .Include(x => x.JobTitle)
                         .Include(x => x.JobGrade)
                         .AsNoTracking().ToListAsync();
    }
}