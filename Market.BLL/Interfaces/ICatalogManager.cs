using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Market.BLL.DTO;

namespace Market.BLL.Interfaces
{
    public interface ICatalogManager : IDisposable
    {
        Task<IEnumerable<string>> Categories();

        Task<CatalogFacetsCriteriesDTO> FacetsCriteries(string categoryName = null);

        Task<ProductsListDTO> Catalog(CatalogFiltersDTO filters);

        Task<ProductsListDTO> SearchByNameOrDescription(CatalogSearchDTO search);

        Task<ProductDTO> Product(int id);
    }
}