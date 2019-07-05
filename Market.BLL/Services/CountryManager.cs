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
    public class CountryManager : ICountryManager
    {
        private readonly IUnitOfWork _database;

        public CountryManager(IUnitOfWork database)
        {
            _database = database;
        }

        public async Task<IEnumerable<CountryDTO>> Countries(string name = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return await _database.Countries.Select(c => new CountryDTO
                {
                    Id = c.Id,
                    Name = c.Name

                }).ToArrayAsync();
            }

            IEnumerable<CountryDTO> countries = await _database.Countries.Select(c => new CountryDTO
            {
                Id = c.Id,
                Name = c.Name

            }).Where(c => c.Name.Contains(name)).ToArrayAsync();

            return countries;
        }

        public async Task<CountryDTO> GetAsync(int id)
        {
            Country country = await _database.Countries.GetAsync(id);

            if (country == null || country.Id != id)
            {
                return null;
            }

            return new CountryDTO
            {
                Id = country.Id,
                Name = country.Name
            };
        }

        public async Task<OperationResult> CreateAsync(string name)
        {
            if (!await CountryNotExists(name))
            {
                return new OperationResult(ResultType.Error, "Country already exists");
            }

            await _database.Countries.CreateAsync(new Country
            {
                Name = name
            });
            await _database.SaveChangesAsync();

            return new OperationResult(ResultType.Success);
        }

        public async Task<OperationResult> Edit(CountryDTO country)
        {
            if (country == null
                || !await _database.Countries.Select(c => c.Id).ContainsAsync(country.Id))
            {
                return new OperationResult(ResultType.Error, "Country doesn't exists");
            }

            if (!await CountryNotExists(country.Name))
            {
                return new OperationResult(ResultType.Error, "Country already exists");
            }

            _database.Countries.Update(new Country
            {
                Id = country.Id,
                Name = country.Name
            });
            await _database.SaveChangesAsync();

            return new OperationResult(ResultType.Success);
        }

        public async Task<OperationResult> Delete(int id)
        {
            Country country = await _database.Countries.GetAsync(id);

            if (country == null || country.Id != id)
            {
                return new OperationResult(ResultType.Warning, "Country doesn't exists");
            }

            _database.Countries.Delete(country);
            await _database.SaveChangesAsync();

            bool countryExists = await _database.Countries
                .Select(c => c.Id).ContainsAsync(id);

            return countryExists
                ? new OperationResult(ResultType.Error, "Could not delete country")
                : new OperationResult(ResultType.Success);
        }

        public async Task<bool> CountryNotExists(string name)
        {
            bool countryNotExists = !await _database.Countries
                .Select(c => c.Name).ContainsAsync(name);

            return countryNotExists;
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