using Domain.Entites;
using SharedKernal;
using Application.Cache;
using Application.Read.ViewModels;

namespace Application.Read.Providers
{
   public abstract class LookupProvider<TEntity, TView> where TEntity : class
    {
        protected readonly CacheProvider<TEntity> _cache;

        public LookupProvider(CacheProvider<TEntity> cache)
        {
            _cache = cache;
        }

        public virtual async Task<Result<List<TView>>> GetAsync(Error emptyError)
        {
            return await GetFromCache(() => _cache.GetAllAsync(), emptyError);
        }

        protected async Task<Result<List<TView>>> GetFromCache(
            Func<Task<List<TEntity>>> fetchCache,
            Error emptyError)
        {
            var data = await fetchCache();

            if (data == null || !data.Any())
                return Result<List<TView>>.Failure(emptyError);

            var views = data.Select(entity => Mapping(entity)).ToList();
            
            return Result<List<TView>>.Successful(views);
        }

        protected abstract TView Mapping(TEntity entity);
    }







    public class DepartmentProvider : LookupProvider<Department, DepartmentView>
    {
        public DepartmentProvider(CacheProvider<Department> cache) : base(cache){}

        public async Task<Result<List<DepartmentView>>> GetAll() 
            => await GetAsync(new Error("DEPARTMENTS_EMPTY", enErrorType.Validation));

        protected override DepartmentView Mapping(Department entity)
        {
            return new DepartmentView
            {
                Id = entity.DepartmentId,
                Name = entity.DepartmentName,
                Code = entity.DepartmentCode,
                Description = entity.DepartmentDescription
            };
        }
    }

    public class NationalityProvider : LookupProvider<Nationality, NationalityView>
    {
        public NationalityProvider(CacheProvider<Nationality> cache) : base(cache){}

        public async Task<Result<List<NationalityView>>> GetAll() 
            => await GetAsync(new Error("NATIONALITIES_EMPTY", enErrorType.Validation));

        protected override NationalityView Mapping(Nationality entity)
        {
            return new NationalityView
            {
                Id = entity.NationalityId,
                Name = entity.NationalityName
            };
        }
    }

    public class JobTitleProvider : LookupProvider<JobTitle, JobTitleView>
    {
        public JobTitleProvider(CacheProvider<JobTitle> cache) : base(cache){}

        public async Task<Result<List<JobTitleView>>> GetAll() 
            => await GetAsync(new Error("JOB_TITLES_EMPTY", enErrorType.Validation));

        protected override JobTitleView Mapping(JobTitle entity)
        {
            return new JobTitleView
            {
                Id = entity.JobTitleId,
                Name = entity.JobTitleName,
                Code = entity.JobTitleCode,
                Description = entity.JobTitleDescription
            };
        }
    }

    public class JobGradeProvider : LookupProvider<JobGrade, JobGradeView>
    {
        public JobGradeProvider(CacheProvider<JobGrade> cache) : base(cache){}

        public async Task<Result<List<JobGradeView>>> GetAll() 
            => await GetAsync(new Error("JOB_GRADES_EMPTY", enErrorType.Validation));

        protected override JobGradeView Mapping(JobGrade entity)
        {
            return new JobGradeView
            {
                Id = entity.JobGradeId,
                Name = entity.JobGradeName,
                Code = entity.GradeCode,
                Description = entity.LevelDescription,
                Weigth = entity.Weight
            };
        }
    }

    public class JobTitleLevelProvider : LookupProvider<JobTitleLevel, JobTitleLevelView>
    {
        public JobTitleLevelProvider(CacheProvider<JobTitleLevel> cache) : base(cache){}

        public async Task<Result<List<JobTitleLevelView>>> GetAll() 
            => await GetAsync(new Error("JOB_LEVELS_EMPTY", enErrorType.Validation));

        protected override JobTitleLevelView Mapping(JobTitleLevel entity)
        {
            var title = entity.JobTitle?.JobTitleName ?? "N/A";
            var grade = entity.JobGrade?.JobGradeName ?? "N/A";
            var weight = entity.JobGrade?.Weight ?? 0;

            return new JobTitleLevelView
            {
                Id = entity.JobTitleLevelId,
                FullTitle = $"{title} - {grade}",
                GradeWeight = weight
            };
        }
    }
}