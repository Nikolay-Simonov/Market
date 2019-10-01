using System.Threading.Tasks;

namespace Market.DAL.Interfaces
{
    public interface ITempStorage<TEntity>
    {
        Task Set(TEntity value);

        Task<TEntity> Get();

        Task Clear();
    }
}