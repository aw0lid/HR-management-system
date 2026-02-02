public interface IDataLoader<TEntity> where TEntity : class
{
    Task<List<TEntity>> GetAsync();
}