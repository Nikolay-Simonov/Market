using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Market.BLL.DTO;
using Market.BLL.Interfaces;
using Market.DAL.Enums;
using Market.DAL.Results;
using Market.Models;
using Market.Models.ProductsManagmentViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
//using NToastNotify;

namespace Market.Controllers
{
    [AutoValidateAntiforgeryToken, Authorize(Roles = "Admin,ContentManager")]
    public class ProductsManagmentController : Controller
    {
        private readonly IProductManager _productManager;
        //private readonly IToastNotification _toast;

        public ProductsManagmentController(IProductManager productManager)//, IToastNotification toastNotification)
        {
            //_toast = toastNotification;
            _productManager = productManager;
        }

        public int PageSize { get; set; } = 15;

        [HttpGet]
        public async Task<ActionResult> List(ProductsListVM productsList, uint page)
        {
            productsList ??= new ProductsListVM();
            productsList.Filters ??= new ProductsFiltersVM();

            var productsTask = _productManager.Products(new ProductsFiltersDTO
            {
                Brand = productsList.Filters.Brand,
                Category = productsList.Filters.Category,
                Country = productsList.Filters.Country,
                Name = productsList.Filters.Name,
                Price = productsList.Filters.Price,
                Removed = productsList.Filters.Removed,
                Weight = productsList.Filters.Weight,
                PageSize = PageSize,
                Page = page
            });
            var brandTask = _productManager.Brands();
            var categorTask = _productManager.Categories();
            var countryTask = _productManager.Countries();

            await Task.WhenAll(productsTask, brandTask, categorTask, countryTask);

            var productsListDTO = await productsTask;
            productsList.Brands = new SelectList((await brandTask).Select(b => b.Name));
            productsList.Categories = new SelectList((await categorTask).Select(c => c.Name));
            productsList.Countries = new SelectList((await countryTask).Select(c => c.Name));
            productsList.Products = productsListDTO.Products.Select(p => new ProductDataVM
            {
                Id = p.Id,
                Brand = p.Brand?.Name,
                Category = p.Category?.Name,
                Country = p.Country.Name,
                Name = p.Name,
                Price = p.Price,
                Removed = p.Removed,
                Weight = p.Weight
            });
            productsList.PagingInfo = new PagingInfoVM
            {
                CurrentPage = productsListDTO.PagingInfo.CurrentPage,
                TotalItems = productsListDTO.PagingInfo.CurrentPage,
                ItemsPerPage = PageSize
            };

            return View(productsList);
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {
            ProductDTO product = await _productManager.GetAsync(id);

            if (product == null || product.Id != id)
            {
                return NotFound();
            }

            var editModel = new ProductCreateVM
            {
                Product = new ProductVM
                {
                    Id = product.Id,
                    CountryId = product.CountryId,
                    CategoryId = product.CategoryId,
                    BrandId = product.BrandId,
                    Name = product.Name,
                    Price = product.Price,
                    Removed = product.Removed,
                    Weight = product.Weight,
                    Character = product.Character,
                    Description = product.Description,
                    Image = product.Image
                },
                Characteristics = _productManager.GetCharacteristicsFromXml(product.Character)
            };

            editModel = await SetDictionaries(editModel);

            return View(editModel);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(ProductCreateVM editModel)
        {
            if (editModel?.Product == null)
            {
                ModelState.AddModelError(string.Empty, "Product not found");
                editModel = await SetDictionaries(editModel ?? new ProductCreateVM());
                return View(editModel);
            }

            editModel = await SetDictionaries(editModel);

            if (!ModelState.IsValid)
            {
                if (editModel.Characteristics == null)
                {
                    return View(editModel);
                }

                ValidateCharacteristics(editModel.Characteristics);

                return View(editModel);
            }

            OperationResult result = await _productManager.Edit(new ProductCreateDTO
            {
                ProductCharacteristics = editModel.Characteristics,
                Product = new ProductDTO
                {
                    Id = editModel.Product.Id,
                    CountryId = editModel.Product.CountryId,
                    CategoryId = editModel.Product.CategoryId,
                    BrandId = editModel.Product.BrandId,
                    Description = editModel.Product.Description,
                    Name = editModel.Product.Name,
                    Price = editModel.Product.Price,
                    Removed = editModel.Product.Removed,
                    Weight = editModel.Product.Weight
                },
                ProductImage = editModel.Image
            });

            // var toastOpt = new NotyOptions
            // {
            //     Timeout = 15000,
            //     ProgressBar = true
            // };

            // if (result.Type == ResultType.Success)
            // {
            //     _toast.AddSuccessToastMessage("Product successfully edited.", toastOpt);
            // }
            // else if (result.Type == ResultType.Warning)
            // {
            //     string warMsg = result.Any(s => !string.IsNullOrWhiteSpace(s))
            //         ? result.BuildMessage()
            //         : "The product has been edited with warnings.";
            //     _toast.AddWarningToastMessage(warMsg, toastOpt);
            // }
            // else if (result.Type == ResultType.Error)
            // {
            //     string errMsg = result.Any(s => !string.IsNullOrWhiteSpace(s))
            //         ? result.BuildMessage()
            //         : "The product has not been edited due to an unknown error.";
            //     _toast.AddErrorToastMessage(errMsg, toastOpt);
            // }
            if (result.Type == ResultType.Success)
            {
                return RedirectToAction(nameof(List));
            }

            if (result.All(string.IsNullOrWhiteSpace))
            {
                string msg = "The product has been edited with warnings.";

                if (result.Type == ResultType.Error)
                {
                    msg = "The product has not been edited due an uknown error.";
                }

                result.Append(msg);
            }

            AddMessages(result.ToArray());

            return View(editModel);
        }

        [HttpGet]
        public async Task<ActionResult> Create()
        {
            return View(await SetDictionaries(new ProductCreateVM()));
        }

        [HttpPost]
        public async Task<ActionResult> Create(ProductCreateVM createModel)
        {
            if (createModel?.Product == null)
            {
                ModelState.AddModelError(string.Empty, "Please fill in the required fields");
                createModel = await SetDictionaries(createModel ?? new ProductCreateVM());
                return View(createModel);
            }

            createModel = await SetDictionaries(createModel);

            if (!ModelState.IsValid)
            {
                if (createModel.Characteristics == null)
                {
                    return View(createModel);
                }

                ValidateCharacteristics(createModel.Characteristics);

                return View(createModel);
            }

            OperationResult result = await _productManager.CreateAsync(new ProductCreateDTO
            {
                ProductCharacteristics = createModel.Characteristics,
                Product = new ProductDTO
                {
                    Id = createModel.Product.Id,
                    CountryId = createModel.Product.CountryId,
                    CategoryId = createModel.Product.CategoryId,
                    BrandId = createModel.Product.BrandId,
                    Description = createModel.Product.Description,
                    Name = createModel.Product.Name,
                    Price = createModel.Product.Price,
                    Removed = createModel.Product.Removed,
                    Weight = createModel.Product.Weight
                },
                ProductImage = createModel.Image
            });

            // var toastOpt = new NotyOptions
            // {
            //     Timeout = 15000,
            //     ProgressBar = true
            // };
            //
            // if (result.Type == ResultType.Success)
            // {
            //     toastOpt.Timeout = 5000;
            //     _toast.AddSuccessToastMessage("Product successfully created.", toastOpt);
            // }
            // else if (result.Type == ResultType.Warning)
            // {
            //     string warMsg = result.Any(s => !string.IsNullOrWhiteSpace(s))
            //         ? result.BuildMessage()
            //         : "The product has been created with warnings.";
            //     _toast.AddWarningToastMessage(warMsg, toastOpt);
            // }

            if (result.Type == ResultType.Success)
            {
                return RedirectToAction(nameof(List));
            }

            if (result.All(string.IsNullOrWhiteSpace))
            {
                result.Append("The product has been created with warnings.");
            }

            AddMessages(result.ToArray());

            return View(createModel);
        }

        [HttpPost]
        public async Task<JsonResult> Remove(int id)
        {
            OperationResult result = await _productManager.Remove(id);

            var messageType = "success";
            var title = "Completed";
            var message = "Product successfully removed from the market catalog.";

            if (result.Type == ResultType.Success)
            {
                return Json(new { messageType, title, message });
            }

            messageType = "error";
            title = "Attention";
            message = result.Any(s => !string.IsNullOrWhiteSpace(s))
                ? result.BuildMessage()
                : "The product has not been removed due to an unknown error.";

            return Json(new { messageType, title, message });
        }

        [HttpPost]
        public async Task<JsonResult> Restore(int id)
        {
            OperationResult result = await _productManager.Restore(id);

            var messageType = "success";
            var title = "Completed";
            var message = "Product successfully restored.";

            if (result.Type == ResultType.Success)
            {
                return Json(new { messageType, title, message });
            }

            messageType = "error";
            title = "Attention";
            message = result.Any(s => !string.IsNullOrWhiteSpace(s))
                ? result.BuildMessage()
                : "The product has not been restored to an uknown error.";

            return Json(new { messageType, title, message });
        }

        private async Task<ProductCreateVM> SetDictionaries(ProductCreateVM model)
        {
            const string id = "Id";
            const string name = "Name";

            var characterTask = _productManager.Characteristics();
            var brandTask = _productManager.Brands();
            var categorTask = _productManager.Categories();
            var countryTask = _productManager.Countries();

            await Task.WhenAll(characterTask, brandTask, categorTask, countryTask);

            model.AvailableCharacteristics = new SelectList(await characterTask, id, name);
            model.Brands = new SelectList(await brandTask, id, name);
            model.Categories = new SelectList(await categorTask, id, name);
            model.Countries = new SelectList(await countryTask, id, name);

            return model;
        }

        private void AddMessages(params string[] messages)
        {
            if (messages == null || messages.Length < 1)
            {
                return;
            }

            foreach (string msg in messages)
            {
                ModelState.AddModelError(string.Empty, msg);
            }
        }

        private void ValidateCharacteristics(IDictionary<string, string> characteristics)
        {
            foreach (var (key, value) in characteristics)
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    ModelState.AddModelError(
                        string.Empty,
                        $"The \"{key}\" characteristic is required."
                    );
                }
            }
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
                _productManager.Dispose();
            }

            base.Dispose(disposing);
            _disposedValue = true;
        }

        #endregion
    }
}