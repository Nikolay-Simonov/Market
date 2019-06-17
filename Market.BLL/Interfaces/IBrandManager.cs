using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Market.BLL.DTO;
using Market.DAL.Results;

namespace Market.BLL.Interfaces
{
    public interface IBrandManager : IDisposable
    {
        Task<IEnumerable<BrandDTO>> Brands(string name = null);

        Task<BrandDTO> GetAsync(int id);

        Task<OperationResult> CreateAsync(string name);

        Task<OperationResult> Edit(BrandDTO country);

        Task<OperationResult> Delete(int id);

        Task<bool> BrandNotExists(string name);
    }
}