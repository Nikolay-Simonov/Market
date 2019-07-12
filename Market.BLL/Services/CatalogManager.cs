using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Market.BLL.DTO;
using Market.BLL.Enums;
using Market.BLL.Interfaces;
using Market.DAL.Entities;
using Market.DAL.Enums;
using Market.DAL.Extensions.Product;
using Market.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Market.BLL.Services
{
    internal class CatalogManager : AppManagerBase, ICatalogManager
    {
        private readonly IFacetsBuilder<Product> _facetsBuilder;

        public CatalogManager(IUnitOfWork database, IFacetsBuilder<Product> facetsBuilder) : base(database)
        {
            _facetsBuilder = facetsBuilder;
        }

        public async Task<IEnumerable<string>> Categories() =>
            await Database.Categories.Select(c => c.Name).ToArrayAsync();

        public async Task<CatalogFacetsCriteriesDTO> FacetsCriteries(string categoryName = null)
        {
            IQueryable<Product> products = Database.Catalog.Products;

            if (!string.IsNullOrWhiteSpace(categoryName))
            {
                products = products.Where(p => p.Category.Name == categoryName);
            }

            var criteries = await products
            .Select(p => new
            {
                BrandName = p.Brand.Name,
                CountryName = p.Country.Name,
                p.Price,
                p.Weight
            })
            .ToArrayAsync();

            decimal maxPrice = 0;
            decimal minPrice = 0;
            double maxWeight = 0;
            double minWeight = 0;

            if (criteries.Any())
            {
                maxPrice = criteries.Select(c => c.Price).Max();
                minPrice = criteries.Select(c => c.Price).Min();
                maxWeight = criteries.Select(c => c.Weight).Max();
                minWeight = criteries.Select(c => c.Weight).Min();
            }

            return new CatalogFacetsCriteriesDTO
            {
                Brands = new HashSet<string>(criteries.Select(c => c.BrandName)),
                Countries = new HashSet<string>(criteries.Select(c => c.CountryName)),
                Characteristics = await Characteristics(categoryName),
                MaxPrice = maxPrice,
                MaxWeight = maxWeight,
                MinPrice = minPrice,
                MinWeight = minWeight
            };
        }

        /// <summary>
        /// Поиск по имени или описанию продукта.
        /// </summary>
        /// <param name="search">ДТО поиска в каталоге.</param>
        public async Task<ProductsListDTO> SearchByNameOrDescription(CatalogSearchDTO search)
        {
            if (search == null || string.IsNullOrWhiteSpace(search.NameOrDescription))
            {
                return new ProductsListDTO
                {
                    Products = new List<ProductDTO>(),
                    PagingInfo = new PagingInfoDTO()
                };
            }

            if (search.Page < 1)
            {
                search.Page = 1;
            }

            if (search.PageSize < 1)
            {
                search.PageSize = 1;
            }

            IQueryable<Product> products = Database.Catalog.Products.Where(p =>
                p.Name.Contains(search.NameOrDescription) || p.Description.Contains(search.NameOrDescription)
            );
            int skipCount = (int)(search.Page - 1) * search.PageSize;
            int totalItems = await products.CountAsync();

            return new ProductsListDTO
            {
                PagingInfo = new PagingInfoDTO
                {
                    CurrentPage = (int)search.Page,
                    ItemsPerPage = search.PageSize,
                    TotalItems = totalItems
                },
                Products = await products.Select(p => new ProductDTO
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
                }).OrderBy(p => p.Id).Skip(skipCount).Take(search.PageSize).ToArrayAsync()
            };
        }

        public async Task<ProductsListDTO> Catalog(CatalogFiltersDTO filters)
        {
            if (filters == null)
            {
                IQueryable<Product> products1 = Database.Catalog.Products.Where(p => !p.Removed);
                int totalItems1 = await products1.CountAsync();
                ProductDTO[] productsDto = await MapToDto(products1, 0, 15,
                    CatalogSortField.Price, SortingDirection.Ascending); // скипаем 0 и берем 15

                return new ProductsListDTO
                {
                    PagingInfo = new PagingInfoDTO
                    {
                        CurrentPage = 1,
                        ItemsPerPage = 15,
                        TotalItems = totalItems1
                    },
                    Products = productsDto
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

            _facetsBuilder.Condition(p => p.Removed, false)
                .And(p => p.Category.Name, filters.Category)
                .And(p => p.Price, filters.StartPrice, Op.GreaterEqual)
                .And(p => p.Price, filters.EndPrice, Op.LessEqual)
                .And(p => p.Weight, filters.StartWeight, Op.GreaterEqual)
                .And(p => p.Weight, filters.EndWeight, Op.LessEqual)
                .AndIn(p => p.Brand.Name, filters.Brands)
                .AndIn(p => p.Country.Name, filters.Countries)
                .AndCharacteristics(filters.Characteristics);

            int skipCount = (int)(filters.Page - 1) * filters.PageSize;
            IQueryable<Product> products2 = Database.Catalog.FacetsSearch(_facetsBuilder);
            int totalItems2 = await products2.CountAsync();
            ProductDTO[] productsDto2 = await MapToDto(
                products2,
                skipCount,
                filters.PageSize,
                filters.SortField,
                filters.SortingDirection
            );

            return new ProductsListDTO
            {
                PagingInfo = new PagingInfoDTO
                {
                    CurrentPage = (int)filters.Page,
                    ItemsPerPage = filters.PageSize,
                    TotalItems = totalItems2
                },
                Products = productsDto2
            };

            #region Local Functions

            static async Task<ProductDTO[]> MapToDto(IQueryable<Product> products, int skip, int pageSize,
                CatalogSortField sortField, SortingDirection sortingDirection)
            {
                IQueryable<ProductDTO> dto = products.Select(p => new ProductDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    BrandId = p.BrandId,
                    CategoryId = p.BrandId,
                    CountryId = p.CountryId,
                    Character = p.Character,
                    Description = p.Description,
                    Image = p.Image,
                    Price = p.Price,
                    Weight = p.Weight,
                    Brand = p.Brand == null
                        ? null
                        : new BrandDTO
                        {
                            Id = p.Brand.Id,
                            Name = p.Brand.Name
                        },
                    Category = p.Category == null
                        ? null
                        : new CategoryDTO
                        {
                            Id = p.Category.Id,
                            Name = p.Category.Name
                        },
                    Country = p.Country == null
                        ? null
                        : new CountryDTO
                        {
                            Id = p.Country.Id,
                            Name = p.Country.Name
                        }
                });

                switch (sortField)
                {
                    case CatalogSortField.Name when sortingDirection == SortingDirection.Descending:
                        dto = dto.OrderByDescending(p => p.Name);
                        break;
                    case CatalogSortField.Name when sortingDirection == SortingDirection.Ascending:
                        dto = dto.OrderBy(p => p.Name);
                        break;
                    case CatalogSortField.Price when sortingDirection == SortingDirection.Descending:
                        dto = dto.OrderByDescending(p => p.Price);
                        break;
                    case CatalogSortField.Price when sortingDirection == SortingDirection.Ascending:
                        dto = dto.OrderBy(p => p.Price);
                        break;
                    case CatalogSortField.Weight when sortingDirection == SortingDirection.Descending:
                        dto = dto.OrderByDescending(p => p.Weight);
                        break;
                    case CatalogSortField.Weight when sortingDirection == SortingDirection.Ascending:
                        dto = dto.OrderBy(p => p.Weight);
                        break;
                }

                return await dto.Skip(skip).Take(pageSize).ToArrayAsync();
            }

            #endregion
        }

        public async Task<ProductDTO> Product(int id)
        {
            return await Database.Catalog.Products.Select(p => new ProductDTO
            {
                Id = p.Id,
                BrandId = p.BrandId,
                CategoryId = p.CategoryId,
                CountryId = p.CountryId,
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
                },
                Character = p.Character,
                Name = p.Name,
                Weight = p.Weight,
                Description = p.Description,
                Image = p.Image,
                Price = p.Price
            }).FirstOrDefaultAsync(p => p.Id == id);
        }

        /// <summary>
        /// Возвращает список всех характеристик продуктов указанной категории или, если категория не указна
        /// то харакретистики всех продуктов.
        /// </summary>
        private async Task<Dictionary<string, HashSet<string>>> Characteristics(string categoryName = null)
        {
            string xml = await Database.Catalog.GetXmlCharacteristics(categoryName);

            if (string.IsNullOrWhiteSpace(xml))
            {
                return new Dictionary<string, HashSet<string>>();
            }

            var root = XElement.Parse(xml);

            if (root.Name != "characteristics")
            {
                return new Dictionary<string, HashSet<string>>();
            }

            Dictionary<string, HashSet<string>> characteristics = root.Elements("characteristic")
                .Where(c => !c.IsEmpty
                            && c.Elements("name").Count(n => !string.IsNullOrWhiteSpace(n.Value)) == 1
                            && c.Elements("value").Count(v => !string.IsNullOrWhiteSpace(v.Value)) == 1)
                .GroupBy(c => c.Element("name").Value, c => c.Element("value").Value)
                .ToDictionary(g => g.Key, g => new HashSet<string>(g));

            return characteristics;
        }
    }
}