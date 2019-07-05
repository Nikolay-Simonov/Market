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
    public class BrandManager : IBrandManager
    {
        private readonly IUnitOfWork _database;

        public BrandManager(IUnitOfWork database)
        {
            _database = database;
        }

        public async Task<IEnumerable<BrandDTO>> Brands(string name = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return await _database.Brands.Select(b => new BrandDTO
                {
                    Id = b.Id,
                    Name = b.Name

                }).ToArrayAsync();
            }

            IEnumerable<BrandDTO> brands = await _database.Brands.Select(b => new BrandDTO
            {
                Id = b.Id,
                Name = b.Name

            }).Where(b => b.Name.Contains(name)).ToArrayAsync();

            return brands;
        }

        public async Task<BrandDTO> GetAsync(int id)
        {
            Brand brand = await _database.Brands.GetAsync(id);

            if (brand == null || brand.Id != id)
            {
                return null;
            }

            return new BrandDTO
            {
                Id = brand.Id,
                Name = brand.Name
            };
        }

        public async Task<OperationResult> CreateAsync(string name)
        {
            if (!await BrandNotExists(name))
            {
                return new OperationResult(ResultType.Error, "Brand already exists");
            }

            await _database.Brands.CreateAsync(new Brand
            {
                Name = name
            });
            await _database.SaveChangesAsync();

            return new OperationResult(ResultType.Success);
        }

        public async Task<OperationResult> Edit(BrandDTO brand)
        {
            if (brand == null
                || !await _database.Brands.Select(b => b.Id).ContainsAsync(brand.Id))
            {
                return new OperationResult(ResultType.Error, "Brand doesn't exists");
            }

            if (!await BrandNotExists(brand.Name))
            {
                return new OperationResult(ResultType.Error, "Brand already exists");
            }

            _database.Brands.Update(new Brand
            {
                Id = brand.Id,
                Name = brand.Name
            });
            await _database.SaveChangesAsync();

            return new OperationResult(ResultType.Success);
        }

        public async Task<OperationResult> Delete(int id)
        {
            Brand brand = await _database.Brands.GetAsync(id);

            if (brand == null || brand.Id != id)
            {
                return new OperationResult(ResultType.Warning, "Brand doesn't exists");
            }

            _database.Brands.Delete(brand);
            await _database.SaveChangesAsync();

            bool brandExists = await _database.Brands
                .Select(b => b.Id).ContainsAsync(id);

            return brandExists
                ? new OperationResult(ResultType.Error, "Could not delete brand")
                : new OperationResult(ResultType.Success);
        }

        public async Task<bool> BrandNotExists(string name)
        {
            bool brandNotExists = !await _database.Brands
                .Select(b => b.Name).ContainsAsync(name);

            return brandNotExists;
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