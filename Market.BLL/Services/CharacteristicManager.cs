using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Market.BLL.DTO;
using Market.BLL.Interfaces;
using Market.DAL.Entities;
using Market.DAL.Enums;
using Market.DAL.Interfaces;
using Market.DAL.Results;
using Microsoft.EntityFrameworkCore;

namespace Market.BLL.Services
{
    public class CharacteristicManager : ICharacteristicManager
    {
        private readonly IUnitOfWork _database;

        public CharacteristicManager(IUnitOfWork database)
        {
            _database = database;
        }

        public async Task<IEnumerable<CharacteristicDTO>> Characteristics(string name = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return await _database.Characteristics.Items.Select(c => new CharacteristicDTO
                {
                    Id = c.Id,
                    Name = c.Name

                }).ToArrayAsync();
            }

            IEnumerable<CharacteristicDTO> characteristics = await _database.Characteristics.Items.Select(c => new CharacteristicDTO
            {
                Id = c.Id,
                Name = c.Name

            }).Where(c => c.Name.Contains(name)).ToArrayAsync();

            return characteristics;
        }

        public async Task<CharacteristicDTO> GetAsync(int id)
        {
            Characteristic characteristic = await _database.Characteristics.GetAsync(id);

            if (characteristic == null || characteristic.Id != id)
            {
                return null;
            }

            return new CharacteristicDTO
            {
                Id = characteristic.Id,
                Name = characteristic.Name
            };
        }

        public async Task<OperationResult> CreateAsync(string name)
        {
            if (!await CharacteristicNotExists(name))
            {
                return new OperationResult(ResultType.Error, "Characteristic already exists");
            }

            await _database.Characteristics.CreateAsync(new Characteristic
            {
                Name = name
            });
            await _database.SaveChangesAsync();

            return new OperationResult(ResultType.Success);
        }

        public async Task<OperationResult> Edit(CharacteristicDTO characteristic)
        {
            if (characteristic == null
                || !await _database.Characteristics.Items.Select(c => c.Id).ContainsAsync(characteristic.Id))
            {
                return new OperationResult(ResultType.Error, "Characteristic doesn't exists");
            }

            if (!await CharacteristicNotExists(characteristic.Name))
            {
                return new OperationResult(ResultType.Error, "Characteristic already exists");
            }

            _database.Characteristics.Update(new Characteristic
            {
                Id = characteristic.Id,
                Name = characteristic.Name
            });
            await _database.SaveChangesAsync();

            return new OperationResult(ResultType.Success);
        }

        public async Task<OperationResult> Delete(int id)
        {
            Characteristic characteristic = await _database.Characteristics.GetAsync(id);

            if (characteristic == null || characteristic.Id != id)
            {
                return new OperationResult(ResultType.Warning, "Characteristic doesn't exists");
            }

            _database.Characteristics.Delete(characteristic);
            await _database.SaveChangesAsync();

            bool characteristicExists = await _database.Characteristics.Items
                .Select(c => c.Id).ContainsAsync(id);

            return characteristicExists
                ? new OperationResult(ResultType.Error, "Could not delete characteristic")
                : new OperationResult(ResultType.Success);
        }

        public async Task<bool> CharacteristicNotExists(string name)
        {
            bool characteristicNotExists = !await _database.Characteristics.Items
                .Select(c => c.Name).ContainsAsync(name);

            return characteristicNotExists;
        }

        #region IDisposable Support

        private bool _disposedValue;

        private void Dispose(bool disposing)
        {
            if (_disposedValue)
            {
                return;
            }

            if (disposing)
            {
                _database.Dispose();
            }

            _disposedValue = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}