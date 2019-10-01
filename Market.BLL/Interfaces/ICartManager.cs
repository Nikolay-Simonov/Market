using Market.BLL.DTO;
using Market.DAL.Results;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Market.BLL.Interfaces
{
    public interface ICartManager : IDisposable
    {
        Task<OperationResult> Add(int id, int quantity = 1);

        Task<OperationResult> Clear();

        Task<ProductLineDTO> ProductLine(int id);

        Task<List<ProductLineDTO>> ProductsLines();

        Task<OperationResult> Remove(int id, int quantity = 1);

        Task<OperationResult> RemoveLine(int id);
    }
}