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
    internal class BrandManager : AppManagerBase, IBrandManager
    {
        public BrandManager(IUnitOfWork database) : base(database) {}

        public async Task<IEnumerable<BrandDTO>> Brands(string name = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return await Database.Brands.Select(b => new BrandDTO
                {
                    Id = b.Id,
                    Name = b.Name

                }).ToArrayAsync();
            }

            IEnumerable<BrandDTO> brands = await Database.Brands.Select(b => new BrandDTO
            {
                Id = b.Id,
                Name = b.Name

            }).Where(b => b.Name.Contains(name)).ToArrayAsync();

            return brands;
        }

        public async Task<BrandDTO> GetAsync(int id)
        {
            Brand brand = await Database.Brands.GetAsync(id);

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

            await Database.Brands.CreateAsync(new Brand
            {
                Name = name
            });
            await Database.SaveChangesAsync();

            return new OperationResult(ResultType.Success);
        }

        public async Task<OperationResult> Edit(BrandDTO brand)
        {
            if (brand == null
                || !await Database.Brands.Select(b => b.Id).ContainsAsync(brand.Id))
            {
                return new OperationResult(ResultType.Error, "Brand doesn't exists");
            }

            if (!await BrandNotExists(brand.Name))
            {
                return new OperationResult(ResultType.Error, "Brand already exists");
            }

            Database.Brands.Update(new Brand
            {
                Id = brand.Id,
                Name = brand.Name
            });
            await Database.SaveChangesAsync();

            return new OperationResult(ResultType.Success);
        }

        public async Task<OperationResult> Delete(int id)
        {
            Brand brand = await Database.Brands.GetAsync(id);

            if (brand == null || brand.Id != id)
            {
                return new OperationResult(ResultType.Warning, "Brand doesn't exists");
            }

            Database.Brands.Delete(brand);
            await Database.SaveChangesAsync();

            bool brandExists = await Database.Brands
                .Select(b => b.Id).ContainsAsync(id);

            return brandExists
                ? new OperationResult(ResultType.Error, "Could not delete brand")
                : new OperationResult(ResultType.Success);
        }

        public async Task<bool> BrandNotExists(string name)
        {
            bool brandNotExists = !await Database.Brands
                .Select(b => b.Name).ContainsAsync(name);

            return brandNotExists;
        }
    }
}