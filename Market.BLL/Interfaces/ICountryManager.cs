using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Market.BLL.DTO;
using Market.DAL.Results;

namespace Market.BLL.Interfaces
{
    public interface ICountryManager : IDisposable
    {
        Task<IEnumerable<CountryDTO>> Countries(string name = null);

        Task<CountryDTO> GetAsync(int id);

        Task<OperationResult> CreateAsync(string name);

        Task<OperationResult> Edit(CountryDTO country);

        Task<OperationResult> Delete(int id);

        Task<bool> CountryNotExists(string name);
    }
}