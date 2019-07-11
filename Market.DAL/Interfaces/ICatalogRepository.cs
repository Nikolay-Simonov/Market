using System.Linq;
using System.Threading.Tasks;
using Market.DAL.Entities;

namespace Market.DAL.Interfaces
{
    public interface ICatalogRepository
    {
        Task<string> GetXmlCharacteristics(string categoryName = null);

        IQueryable<Product> Products { get; }

        IQueryable<Product> FacetsSearch(IFacetsBuilder<Product> facetsBuilder);
    }
}