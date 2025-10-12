
namespace SGBL.Domain.Interfaces
{
    public interface IGenericRepository<Entity> where Entity : class
    {
        Task<Entity> AddAsync(Entity entity);
        IQueryable<Entity> GetAllQueryWithInclude(List<string> properties);
        Task<List<Entity>> GetAllAsyncWithInclude(List<string> properties);
        Task<List<Entity>?> AddRangeAsync(List<Entity> entities);
        Task DeleteAsync(int id);
        Task<List<Entity>> GetAllAsync();
        IQueryable<Entity> GetAllQuery();
        Task<Entity?> GetById(int id);
        Task<Entity?> UpdateAsync(int id, Entity entity);
    }
}
