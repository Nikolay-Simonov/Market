using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Market.BLL.DTO;
using Market.DAL.Results;

namespace Market.BLL.Interfaces
{
    public interface ICharacteristicManager : IDisposable
    {
        Task<IEnumerable<CharacteristicDTO>> Characteristics(string name = null);

        Task<CharacteristicDTO> GetAsync(int id);

        Task<OperationResult> CreateAsync(string name);

        Task<OperationResult> Edit(CharacteristicDTO country);

        Task<OperationResult> Delete(int id);

        Task<bool> CharacteristicNotExists(string name);
    }
}