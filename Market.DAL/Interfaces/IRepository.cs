using System.Linq;
using System.Threading.Tasks;

namespace Market.DAL.Interfaces
{
    public interface IRepository<TEntity> : IQueryable<TEntity> where TEntity : class
    {
        Task<TEntity> GetAsync(int id);

        Task CreateAsync(TEntity item);

        void Update(TEntity item);

        void Delete(TEntity item);
    }
}