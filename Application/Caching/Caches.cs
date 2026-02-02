using Domain.Entites;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Cache
{
    public abstract class CacheProvider<TEntity> where TEntity : class
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly string _cacheKey;
        private readonly int _spanTimeDayes;

        protected CacheProvider(IServiceScopeFactory scopeFactory, IMemoryCache memoryCache, string cacheKey, int spanTimeDayes = 1)
        {
            _memoryCache = memoryCache;
            _scopeFactory = scopeFactory;
            _cacheKey = cacheKey;
            _spanTimeDayes = spanTimeDayes;
        }

        public async Task<List<TEntity>> GetAllAsync()
        {
            return await _memoryCache.GetOrCreateAsync(_cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(_spanTimeDayes);
                
                using (var scope = _scopeFactory.CreateScope())
                {
                    var scopedReader = scope.ServiceProvider.GetRequiredService<IDataLoader<TEntity>>();
                    return await scopedReader.GetAsync();
                }
            }) ?? new List<TEntity>();
        }

        public void Invalidate() => _memoryCache.Remove(_cacheKey);
    }

    public class DepartmentsCache(IServiceScopeFactory scopeFactory, IMemoryCache memoryCache)
        : CacheProvider<Department>(scopeFactory, memoryCache, "DEPARTMENTS_CACHE", spanTimeDayes: 1)
    { }

    public class NationalitiesCache(IServiceScopeFactory scopeFactory, IMemoryCache memoryCache)
        : CacheProvider<Nationality>(scopeFactory, memoryCache, "NATIONALITIES_CACHE", spanTimeDayes: 7)
    { }

    public class JobTitlesCache(IServiceScopeFactory scopeFactory, IMemoryCache memoryCache)
        : CacheProvider<JobTitle>(scopeFactory, memoryCache, "JOB_TITLES_CACHE", spanTimeDayes: 1)
    { }

    public class JobTitleGradesCache(IServiceScopeFactory scopeFactory, IMemoryCache memoryCache)
        : CacheProvider<JobGrade>(scopeFactory, memoryCache, "JOB_TITLE_GRADES_CACHE", spanTimeDayes: 1)
    { }

    public class JobTitleLevelsCache(IServiceScopeFactory scopeFactory, IMemoryCache memoryCache)
        : CacheProvider<JobTitleLevel>(scopeFactory, memoryCache, "JOB_TITLE_LEVELS_CACHE", spanTimeDayes: 1)
    { }
}