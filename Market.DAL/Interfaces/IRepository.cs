using System.Linq;
using System.Threading.Tasks;

namespace Market.DAL.Interfaces
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> Items { get; }

        Task<T> GetAsync(int id);

        Task CreateAsync(T item);

        void Update(T item);

        void Delete(T item);
    }
}