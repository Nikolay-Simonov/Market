using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Market.BLL.DTO;
using Market.BLL.Enums;
using Market.BLL.Interfaces;
using Market.Models;
using Market.Models.CatalogViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Market.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class CatalogController : Controller
    {
        private readonly ICatalogManager _catalogManager;

        public CatalogController(ICatalogManager catalogManager)
        {
            _catalogManager = catalogManager;
        }

        public int PageSize { get; set; } = 15;

        [HttpGet]
        public async Task<ActionResult<ProductVM>> Product(int id)
        {
            ProductDTO productDto = await _catalogManager.Product(id);

            if (productDto == null || productDto.Id != id)
            {
                return NotFound();
            }

            return View(new ProductVM
            {
                Id = productDto.Id,
                Name = productDto.Name,
                Brand = productDto.Brand == null ? null : new BrandVM
                {
                    Id = productDto.Brand.Id,
                    Name = productDto.Brand.Name
                },
                Category = productDto.Category == null ? null : new CategoryVM
                {
                    Id = productDto.Category.Id,
                    Name = productDto.Category.Name
                },
                Character = productDto.Character,
                Country = productDto.Country == null ? null : new CountryVM
                {
                    Id = productDto.Country.Id,
                    Name = productDto.Country.Name
                },
                Description = productDto.Description,
                Image = productDto.Image,
                Price = productDto.Price,
                Weight = productDto.Weight,
                BrandId = productDto.BrandId,
                CategoryId = productDto.CategoryId,
                CountryId = productDto.CountryId
            });
        }

        [HttpGet]
        public async Task<ActionResult<CatalogVM>> Products(CatalogVM catalogVm, uint page)
        {
            CatalogFacetsCriteriesDTO criteriesDto = await _catalogManager.FacetsCriteries(catalogVm?.Category);
            ProductsListDTO productsListDto;

            if (catalogVm == null)
            {
                productsListDto = await _catalogManager.Catalog(null);
                catalogVm = new CatalogVM
                {
                    Category = null,
                    SortField = CatalogSortField.Price,
                    SortingDirection = SortingDirection.Ascending
                };
            }
            else
            {
                productsListDto = await _catalogManager.Catalog(new CatalogFiltersDTO
                {
                    Brands = catalogVm.Brands,
                    Characteristics = catalogVm.Characteristics,
                    Countries = catalogVm.Countries,
                    Category = catalogVm.Category,
                    SortField = catalogVm.SortField,
                    SortingDirection = catalogVm.SortingDirection,
                    EndPrice = catalogVm.EndPrice,
                    EndWeight = catalogVm.EndWeight,
                    StartPrice = catalogVm.StartPrice,
                    StartWeight = catalogVm.StartWeight,
                    PageSize = PageSize,
                    Page = page
                });
            }

            // При одинаковом названии характеристик и свойств продукта выбросится исключение!
            catalogVm.FacetsCriteries = criteriesDto.Characteristics
            .Select(c =>
            {
                HashSet<string> values = null;
                catalogVm.Characteristics?.TryGetValue(c.Key, out values);

                var facetCriterionVm = new FacetCriterionVM(c.Key, nameof(catalogVm.Characteristics))
                {
                    All = c.Value,
                    Selected = values
                };

                return facetCriterionVm;
            })
            .Append(new FacetCriterionVM(nameof(catalogVm.Countries))
            {
                All = criteriesDto.Countries,
                Selected = catalogVm.Countries
            })
            .Append(new FacetCriterionVM(nameof(catalogVm.Brands))
            {
                All = criteriesDto.Brands,
                Selected = catalogVm.Brands
            })
            .ToHashSet();

            catalogVm.MaxPrice = criteriesDto.MaxPrice;
            catalogVm.MaxWeight = criteriesDto.MaxWeight;
            catalogVm.MinPrice = criteriesDto.MinPrice;
            catalogVm.MinWeight = criteriesDto.MinWeight;

            catalogVm.Products = productsListDto.Products.Select(p => new ProductShortVM
            {
                Id = p.Id,
                Name = p.Name,
                Brand = p.Brand?.Name,
                Country = p.Country?.Name,
                Image = p.Image,
                Price = p.Price,
                Weight = p.Weight
            });

            catalogVm.PagingInfo = new PagingInfoVM
            {
                CurrentPage = productsListDto.PagingInfo.CurrentPage,
                TotalItems = productsListDto.PagingInfo.TotalItems,
                ItemsPerPage = productsListDto.PagingInfo.ItemsPerPage
            };

            return View(catalogVm);
        }

        [HttpGet]
        public async Task<ActionResult<SearchResultsVM>> Search(string query, uint page)
        {
            ProductsListDTO productsListDto = await _catalogManager.SearchByNameOrDescription(new CatalogSearchDTO
            {
                Page = page,
                PageSize = PageSize,
                NameOrDescription = query
            });

            return View(new SearchResultsVM
            {
                Products = productsListDto.Products.Select(p => new ProductShortVM
                {
                    Id = p.Id,
                    Brand = p.Brand?.Name,
                    Country = p.Country?.Name,
                    Image = p.Image,
                    Name = p.Name,
                    Price = p.Price,
                    Weight = p.Weight
                }),
                PagingInfo = new PagingInfoVM
                {
                    CurrentPage = productsListDto.PagingInfo.CurrentPage,
                    TotalItems = productsListDto.PagingInfo.TotalItems,
                    ItemsPerPage = productsListDto.PagingInfo.ItemsPerPage
                }
            });
        }

        #region IDisposable Support

        private bool _disposedValue;

        protected override void Dispose(bool disposing)
        {
            if (_disposedValue)
            {
                return;
            }

            if (disposing)
            {
                _catalogManager.Dispose();
            }

            _disposedValue = true;
        }

        #endregion
    }
}