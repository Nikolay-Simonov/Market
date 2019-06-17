using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Market.BLL.DTO;
using Market.DAL.Results;

namespace Market.BLL.Interfaces
{
    public interface ICategoryManager : IDisposable
    {
        Task<IEnumerable<CategoryDTO>> Categories(string name = null);

        Task<CategoryDTO> GetAsync(int id);

        Task<OperationResult> CreateAsync(string name);

        Task<OperationResult> Edit(CategoryDTO country);

        Task<OperationResult> Delete(int id);

        Task<bool> CategoryNotExists(string name);
    }
}