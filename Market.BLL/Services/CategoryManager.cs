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
    internal class CategoryManager: AppManagerBase, ICategoryManager
    {
        public CategoryManager(IUnitOfWork database) : base(database) {}

        public async Task<IEnumerable<CategoryDTO>> Categories(string name = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return await Database.Categories.Select(c => new CategoryDTO
                {
                    Id = c.Id,
                    Name = c.Name

                }).ToArrayAsync();
            }

            IEnumerable<CategoryDTO> categories = await Database.Categories.Select(c => new CategoryDTO
            {
                Id = c.Id,
                Name = c.Name

            }).Where(c => c.Name.Contains(name)).ToArrayAsync();

            return categories;
        }

        public async Task<CategoryDTO> GetAsync(int id)
        {
            Category category = await Database.Categories.GetAsync(id);

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

            await Database.Categories.CreateAsync(new Category
            {
                Name = name
            });
            await Database.SaveChangesAsync();

            return new OperationResult(ResultType.Success);
        }

        public async Task<OperationResult> Edit(CategoryDTO category)
        {
            if (category == null
                || !await Database.Categories.Select(c => c.Id).ContainsAsync(category.Id))
            {
                return new OperationResult(ResultType.Error, "Category doesn't exists");
            }

            if (!await CategoryNotExists(category.Name))
            {
                return new OperationResult(ResultType.Error, "Category already exists");
            }

            Database.Categories.Update(new Category
            {
                Id = category.Id,
                Name = category.Name
            });
            await Database.SaveChangesAsync();

            return new OperationResult(ResultType.Success);
        }

        public async Task<OperationResult> Delete(int id)
        {
            Category category = await Database.Categories.GetAsync(id);

            if (category == null || category.Id != id)
            {
                return new OperationResult(ResultType.Warning, "Category doesn't exists");
            }

            Database.Categories.Delete(category);
            await Database.SaveChangesAsync();

            bool categoryExists = await Database.Categories
                .Select(c => c.Id).ContainsAsync(id);

            return categoryExists
                ? new OperationResult(ResultType.Error, "Could not delete category")
                : new OperationResult(ResultType.Success);
        }

        public async Task<bool> CategoryNotExists(string name)
        {
            bool categoryNotExists = !await Database.Categories
                .Select(c => c.Name).ContainsAsync(name);

            return categoryNotExists;
        }
    }
}