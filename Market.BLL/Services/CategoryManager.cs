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
    public class CategoryManager : ICategoryManager
    {
        private readonly IUnitOfWork _database;

        public CategoryManager(IUnitOfWork database)
        {
            _database = database;
        }

        public async Task<IEnumerable<CategoryDTO>> Categories(string name = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return await _database.Categories.Items.Select(c => new CategoryDTO
                {
                    Id = c.Id,
                    Name = c.Name

                }).ToArrayAsync();
            }

            IEnumerable<CategoryDTO> categories = await _database.Categories.Items.Select(c => new CategoryDTO
            {
                Id = c.Id,
                Name = c.Name

            }).Where(c => c.Name.Contains(name)).ToArrayAsync();

            return categories;
        }

        public async Task<CategoryDTO> GetAsync(int id)
        {
            Category category = await _database.Categories.GetAsync(id);

            if (category == null || category.Id != id)
            {
                return null;
            }

            return new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name
            };
        }

        public async Task<OperationResult> CreateAsync(string name)
        {
            if (!await CategoryNotExists(name))
            {
                return new OperationResult(ResultType.Error, "Category already exists");
            }

            await _database.Categories.CreateAsync(new Category
            {
                Name = name
            });
            await _database.SaveChangesAsync();

            return new OperationResult(ResultType.Success);
        }

        public async Task<OperationResult> Edit(CategoryDTO category)
        {
            if (category == null
                || !await _database.Categories.Items.Select(c => c.Id).ContainsAsync(category.Id))
            {
                return new OperationResult(ResultType.Error, "Category doesn't exists");
            }

            if (!await CategoryNotExists(category.Name))
            {
                return new OperationResult(ResultType.Error, "Category already exists");
            }

            _database.Categories.Update(new Category
            {
                Id = category.Id,
                Name = category.Name
            });
            await _database.SaveChangesAsync();

            return new OperationResult(ResultType.Success);
        }

        public async Task<OperationResult> Delete(int id)
        {
            Category category = await _database.Categories.GetAsync(id);

            if (category == null || category.Id != id)
            {
                return new OperationResult(ResultType.Warning, "Category doesn't exists");
            }

            _database.Categories.Delete(category);
            await _database.SaveChangesAsync();

            bool categoryExists = await _database.Categories.Items
                .Select(c => c.Id).ContainsAsync(id);

            return categoryExists
                ? new OperationResult(ResultType.Error, "Could not delete category")
                : new OperationResult(ResultType.Success);
        }

        public async Task<bool> CategoryNotExists(string name)
        {
            bool categoryNotExists = !await _database.Categories.Items
                .Select(c => c.Name).ContainsAsync(name);

            return categoryNotExists;
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