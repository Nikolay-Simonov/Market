using System.Collections.Generic;
using System.Threading.Tasks;
using Market.DAL.Entities;

namespace Market.DAL.Interfaces
{
    public interface ICartRepository
    {
        Task Clear(string userId);

        Task<ProductLine> CreateAsync(CartLine line);

        Task<ProductLine> ProductLine(int productId, string userId);

        Task<IEnumerable<ProductLine>> ProductsLines(string userId);

        Task Remove(int productId, string userId);

        ProductLine Update(CartLine line);
    }
}