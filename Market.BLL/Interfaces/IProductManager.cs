using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Market.BLL.DTO;
using Market.DAL.Results;

namespace Market.BLL.Interfaces
{
    public interface IProductManager : IDisposable
    {
        Task<IEnumerable<BrandDTO>> Brands();

        Task<IEnumerable<CountryDTO>> Countries();

        Task<IEnumerable<CharacteristicDTO>> Characteristics();

        Task<IEnumerable<CategoryDTO>> Categories();

        Task<ProductsListDTO> Products(ProductsFiltersDTO filters);

        Task<ProductDTO> GetAsync(int id);

        Task<OperationResult> Edit(ProductCreateDTO editModel);

        Task<OperationResult> CreateAsync(ProductCreateDTO createModel);

        Task<OperationResult> Remove(int id);

        Task<OperationResult> Restore(int id);

        Dictionary<string, string> GetCharacteristicsFromXml(string xml);

        string GetCharacteristicsFromDictionary(IDictionary<string, string> characteristics);
    }
}