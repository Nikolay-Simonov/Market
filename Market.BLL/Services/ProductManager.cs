using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Market.BLL.DTO;
using Market.BLL.Interfaces;
using Market.DAL.Entities;
using Market.DAL.Enums;
using Market.DAL.Interfaces;
using Market.DAL.Results;
using Microsoft.EntityFrameworkCore;

namespace Market.BLL.Services
{
    internal class ProductManager : AppManagerBase, IProductManager
    {
        public ProductManager(IUnitOfWork database) : base(database) {}

        public async Task<IEnumerable<BrandDTO>> Brands()
        {
            var brands = await Database.Brands.Select(b => new BrandDTO
            {
                Id = b.Id,
                Name = b.Name

            }).ToArrayAsync();

            return brands;
        }

        public async Task<IEnumerable<CategoryDTO>> Categories()
        {
            var categories = await Database.Categories.Select(c => new CategoryDTO
            {
                Id = c.Id,
                Name = c.Name

            }).ToArrayAsync();

            return categories;
        }

        public async Task<IEnumerable<CountryDTO>> Countries()
        {
            var countries = await Database.Countries.Select(c => new CountryDTO
            {
                Id = c.Id,
                Name = c.Name

            }).ToArrayAsync();

            return countries;
        }

        public async Task<IEnumerable<CharacteristicDTO>> Characteristics()
        {
            var characteristics = await Database.Characteristics.Select(c => new CharacteristicDTO
            {
                Id = c.Id,
                Name = c.Name

            }).ToArrayAsync();

            return characteristics;
        }

        public async Task<ProductsListDTO> Products(ProductsFiltersDTO filters)
        {
            IQueryable<Product> products = Database.Products;

            if (filters == null)
            {
                return new ProductsListDTO
                {
                    PagingInfo = new PagingInfoDTO
                    {
                        CurrentPage = 1,
                        ItemsPerPage = 15,
                        TotalItems = await products.CountAsync()
                    },
                    // Скипаем ноль объектов, и берем 15.
                    Products = await MapToDto(0, 15)
                };
            }

            if (filters.Page < 1)
            {
                filters.Page = 1;
            }

            if (filters.PageSize < 1)
            {
                filters.PageSize = 1;
            }

            if (filters.Removed == RemoveState.Yes)
            {
                products = products.Where(p => p.Removed);
            }
            else if (filters.Removed == RemoveState.No)
            {
                products = products.Where(p => p.Removed == false);
            }

            if (!string.IsNullOrWhiteSpace(filters.Category))
            {
                products = products.Where(p => p.Category.Name.Contains(filters.Category));
            }

            if (!string.IsNullOrWhiteSpace(filters.Brand))
            {
                products = products.Where(p => p.Brand.Name.Contains(filters.Brand));
            }

            if (!string.IsNullOrWhiteSpace(filters.Country))
            {
                products = products.Where(p => p.Country.Name.Contains(filters.Country));
            }

            if (!string.IsNullOrWhiteSpace(filters.Name))
            {
                products = products.Where(p => p.Name.Contains(filters.Name));
            }

            if (filters.Price.HasValue)
            {
                products = products.Where(p => p.Price == filters.Price);
            }

            if (filters.Weight.HasValue)
            {
                products = products.Where(p => p.Weight == filters.Weight);
            }

            int totalItems = await products.CountAsync();
            int skipCount = (int)(filters.Page - 1) * filters.PageSize;

            var productsManagmentList = new ProductsListDTO
            {
                Products = await MapToDto(skipCount, filters.PageSize),
                PagingInfo = new PagingInfoDTO
                {
                    CurrentPage = (int)filters.Page,
                    ItemsPerPage = filters.PageSize,
                    TotalItems = totalItems
                }
            };

            return productsManagmentList;

            #region Local Functions

            async Task<IEnumerable<ProductDTO>> MapToDto(int skip, int pageSize)
            {
                var dtoList = await products.Select(p => new ProductDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    BrandId = p.BrandId,
                    CategoryId = p.CategoryId,
                    CountryId = p.CountryId,
                    Character = p.Character,
                    Description = p.Description,
                    Image = p.Image,
                    Price = p.Price,
                    Weight = p.Weight,
                    Removed = p.Removed,
                    Brand = p.Brand == null ? null : new BrandDTO
                    {
                        Id = p.Brand.Id,
                        Name = p.Brand.Name
                    },
                    Category = p.Category == null ? null : new CategoryDTO
                    {
                        Id = p.Category.Id,
                        Name = p.Category.Name
                    },
                    Country = p.Country == null ? null : new CountryDTO
                    {
                        Id = p.Country.Id,
                        Name = p.Country.Name
                    }

                }).OrderBy(p => p.Id).Skip(skip).Take(pageSize).ToArrayAsync();

                return dtoList;
            }

            #endregion
        }

        public async Task<OperationResult> CreateAsync(ProductCreateDTO createModel)
        {
            List<string> messages = new List<string>();

            var product = new Product
            {
                CategoryId = createModel.Product.CategoryId,
                CountryId = createModel.Product.CountryId,
                BrandId = createModel.Product.BrandId,
                Description = createModel.Product.Description,
                Name = createModel.Product.Name,
                Price = createModel.Product.Price,
                Weight = createModel.Product.Weight
            };

            if (product.CategoryId.HasValue)
            {
                bool categoryExists = await Database.Categories
                    .Select(c => c.Id)
                    .ContainsAsync(product.CategoryId.Value);

                if (!categoryExists)
                {
                    messages.Append("The category does not exist in the system.");
                    product.CategoryId = null;
                }
            }

            if (product.BrandId.HasValue)
            {
                bool brandExists = await Database.Brands
                    .Select(c => c.Id)
                    .ContainsAsync(product.BrandId.Value);

                if (!brandExists)
                {
                    messages.Append("The brand does not exist in the system.");
                    product.BrandId = null;
                }
            }

            if (product.CountryId.HasValue)
            {
                bool countryExists = await Database.Countries
                    .Select(c => c.Id)
                    .ContainsAsync(product.CountryId.Value);

                if (!countryExists)
                {
                    messages.Append("The country does not exist in the system.");
                    product.CountryId = null;
                }
            }

            if (createModel.ProductCharacteristics != null && createModel.ProductCharacteristics.Count > 0)
            {
                List<string> availableChars = await Database.Characteristics
                    .Select(c => c.Name).ToListAsync();

                var validChars = new Dictionary<string, string>();

                foreach (var characteristic in createModel.ProductCharacteristics)
                {
                    if (!availableChars.Contains(characteristic.Key))
                    {
                        messages.Append($"Characteristic {characteristic} does not exists.");
                        continue;
                    }

                    validChars.Add(characteristic.Key, characteristic.Value);
                }

                product.Character = GetCharacteristicsFromDictionary(validChars);
            }

            if (createModel.ProductImage != null && createModel.ProductImage.Length > 0)
            {
                ImageSaveResult saveResult = await Database.ProductsImages.Save(createModel.ProductImage);

                if (saveResult.Type == ResultType.Success && !string.IsNullOrWhiteSpace(saveResult.OutputPath))
                {
                    product.Image = saveResult.OutputPath;
                }
                else if (saveResult.Messages != null && saveResult.Any())
                {
                    messages.AddRange(saveResult.Messages);
                }
            }

            await Database.Products.CreateAsync(product);
            await Database.SaveChangesAsync();

            ResultType type = messages.Any()
                ? ResultType.Warning
                : ResultType.Success;

            return OperationResult(type, messages.ToArray());
        }

        /// <summary>
        /// Изменяет сущность продукта и асинхронно сохраняет ее.
        /// </summary>
        public async Task<OperationResult> Edit(ProductCreateDTO editModel)
        {
            if (editModel?.Product == null ||
                !await Database.Products.Select(p => p.Id).ContainsAsync(editModel.Product.Id))
            {
                return OperationResult(ResultType.Error, "Product not found.");
            }

            List<string> messages = new List<string>();

            var product = new Product
            {
                Id = editModel.Product.Id,
                BrandId = editModel.Product.BrandId,
                CategoryId = editModel.Product.CategoryId,
                CountryId = editModel.Product.CountryId,
                Description = editModel.Product.Description,
                Name = editModel.Product.Name,
                Price = editModel.Product.Price,
                Removed = editModel.Product.Removed,
                Weight = editModel.Product.Weight
            };

            if (product.CategoryId.HasValue)
            {
                bool categoryExists = await Database.Categories
                    .Select(c => c.Id)
                    .ContainsAsync(product.CategoryId.Value);

                if (!categoryExists)
                {
                    messages.Append("The category does not exist in the system.");
                    product.CategoryId = null;
                }
            }

            if (product.BrandId.HasValue)
            {
                bool brandExists = await Database.Brands
                    .Select(c => c.Id)
                    .ContainsAsync(product.BrandId.Value);

                if (!brandExists)
                {
                    messages.Append("The brand does not exist in the system.");
                    product.BrandId = null;
                }
            }

            if (product.CountryId.HasValue)
            {
                bool countryExists = await Database.Countries
                    .Select(c => c.Id)
                    .ContainsAsync(product.CountryId.Value);

                if (!countryExists)
                {
                    messages.Append("The country does not exist in the system.");
                    product.CountryId = null;
                }
            }

            if (editModel.ProductCharacteristics != null && editModel.ProductCharacteristics.Count > 0)
            {
                List<string> availableChars = await Database.Characteristics
                    .Select(c => c.Name).ToListAsync();

                var validChars = new Dictionary<string, string>();

                foreach (var characteristic in editModel.ProductCharacteristics)
                {
                    if (!availableChars.Contains(characteristic.Key))
                    {
                        messages.Append($"Characteristic {characteristic} does not exists.");
                        continue;
                    }

                    validChars.Add(characteristic.Key, characteristic.Value);
                }

                product.Character = GetCharacteristicsFromDictionary(validChars);
            }

            string oldFilePath = await Database.Products.Where(p => p.Id == product.Id)
                .Select(p => p.Image).FirstOrDefaultAsync();

            if (!string.IsNullOrWhiteSpace(oldFilePath))
            {
                OperationResult deleteResult = Database.ProductsImages.Delete(oldFilePath);

                if (deleteResult.Type != ResultType.Success
                    && deleteResult.Messages != null && deleteResult.Any())
                {
                    messages.AddRange(deleteResult);
                }
            }

            if (editModel.ProductImage != null && editModel.ProductImage.Length > 0)
            {
                ImageSaveResult saveResult = await Database.ProductsImages.Save(editModel.ProductImage);

                if (saveResult.Type == ResultType.Success && !string.IsNullOrWhiteSpace(saveResult.OutputPath))
                {
                    product.Image = saveResult.OutputPath;
                }
                else if (saveResult.Messages != null && saveResult.Any())
                {
                    messages.AddRange(saveResult);
                }
            }

            Database.Products.Update(product);
            await Database.SaveChangesAsync();

            ResultType type = messages.Any()
                ? ResultType.Warning
                : ResultType.Success;

            return OperationResult(type, messages.ToArray());
        }

        public async Task<ProductDTO> GetAsync(int id)
        {
            Product product = await Database.Products.GetAsync(id);

            if (product == null || product.Id != id)
            {
                return null;
            }

            var dto = new ProductDTO
            {
                Id = product.Id,
                Name = product.Name,
                BrandId = product.BrandId,
                CategoryId = product.CategoryId,
                CountryId = product.CountryId,
                Image = product.Image,
                Description = product.Description,
                Price = product.Price,
                Removed = product.Removed,
                Character = product.Character,
                Weight = product.Weight
            };

            if (product.Brand != null)
            {
                dto.Brand = new BrandDTO
                {
                    Id = product.Brand.Id,
                    Name = product.Brand.Name
                };
            }

            if (product.Category != null)
            {
                dto.Category = new CategoryDTO
                {
                    Id = product.Category.Id,
                    Name = product.Category.Name
                };
            }

            if (product.Country != null)
            {
                dto.Country = new CountryDTO
                {
                    Id = product.Country.Id,
                    Name = product.Country.Name
                };
            }

            return dto;
        }

        /// <summary>
        /// Помечает продукт как удаленный и производит асинхронное сохранение изменений.
        /// </summary>
        public async Task<OperationResult> Remove(int id)
        {
            Product product = await Database.Products.GetAsync(id);

            if (product == null || product.Id != id || product.Removed)
            {
                return OperationResult(ResultType.Error, "Product not found or already removed.");
            }

            product.Removed = true;

            Database.Products.Update(product);
            await Database.SaveChangesAsync();

            return OperationResult(ResultType.Success);
        }

        /// <summary>
        /// Восстанавливает продукт и производит асинхронное сохранение изменений.
        /// </summary>
        public async Task<OperationResult> Restore(int id)
        {
            Product product = await Database.Products.GetAsync(id);

            if (product == null || product.Id != id || !product.Removed)
            {
                return OperationResult(ResultType.Error, "Product not found or already restored.");
            }

            product.Removed = false;

            Database.Products.Update(product);
            await Database.SaveChangesAsync();

            return OperationResult(ResultType.Success);
        }

        /// <summary>
        /// Возвращает новый словарь характеристик полученный из XML кода.
        /// </summary>
        public Dictionary<string, string> GetCharacteristicsFromXml(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml))
            {
                return new Dictionary<string, string>();
            }

            var root = XElement.Parse(xml);

            if (root.Name != "characteristics")
            {
                return new Dictionary<string, string>();
            }

            Dictionary<string, string> characteristics = root.Elements("characteristic")
                .Where(c => !c.IsEmpty
                            && c.Elements("name").Count(n => !string.IsNullOrWhiteSpace(n.Value)) == 1
                            && c.Elements("value").Count(v => !string.IsNullOrWhiteSpace(v.Value)) == 1)
                .ToDictionary(с => с.Element("name").Value, c => c.Element("value").Value);

            return characteristics;
        }

        /// <summary>
        /// Возвращает XML код из словаря характеристик.
        /// </summary>
        public string GetCharacteristicsFromDictionary(IDictionary<string, string> characteristics)
        {
            if (characteristics == null || !characteristics.Any())
            {
                return string.Empty;
            }

            var root = new XElement("characteristics");

            foreach (var (key, value) in characteristics)
            {
                if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value))
                {
                    continue;
                }

                var characteristic = new XElement("characteristic");
                characteristic.Add(new XElement("name", key));
                characteristic.Add(new XElement("value", value));
                root.Add(characteristic);
            }

            return root.ToString();
        }

        private static OperationResult OperationResult(ResultType type, params string[] messages)
        {
            return new OperationResult(type, messages.ToArray());
        }
    }
}