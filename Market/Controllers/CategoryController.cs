using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Market.BLL.DTO;
using Market.BLL.Interfaces;
using Market.DAL.Enums;
using Market.DAL.Results;
using Market.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
//using NToastNotify;

namespace Market.Controllers
{
    [AutoValidateAntiforgeryToken, Authorize(Roles = "Admin,ContentManager")]
    public class CategoryController : Controller
    {
        private readonly ICategoryManager _categoryManager;
        //private readonly IToastNotification _toast;

        public CategoryController(ICategoryManager categoryManager)//, IToastNotification toast)
        {
            _categoryManager = categoryManager;
            //_toast = toast;
        }

        [HttpGet]
        public async Task<ActionResult> List(string name)
        {
            IEnumerable<CategoryVM> categories = (await _categoryManager.Categories(name)).Select(c => new CategoryVM
            {
                Id = c.Id,
                Name = c.Name
            });

            return View(categories);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(CategoryVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            OperationResult result = await _categoryManager.CreateAsync(model.Name);

            if (result.Type == ResultType.Success)
            {
                //_toast.AddSuccessToastMessage("Category has been successfully created");
                //return View();
                return RedirectToAction(nameof(List));
            }

            ModelState.AddModelError(nameof(model.Name), result.BuildMessage());

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {
            CategoryDTO category = await _categoryManager.GetAsync(id);

            if (category == null)
            {
                return NotFound("Category doesn't exist");
            }

            return View(new CategoryVM
            {
                Id = category.Id,
                Name = category.Name
            });
        }

        [HttpPost]
        public async Task<ActionResult> Edit(CategoryVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            OperationResult result = await _categoryManager.Edit(new CategoryDTO
            {
                Id = model.Id,
                Name = model.Name
            });

            if (result.Type == ResultType.Success)
            {
                //_toast.AddSuccessToastMessage("Category has been successfully edited");
                //return View(model);
                return RedirectToAction(nameof(List));
            }

            ModelState.AddModelError(nameof(model.Name), result.BuildMessage());

            return View(model);
        }

        [HttpPost]
        public async Task<JsonResult> Delete(int id)
        {
            OperationResult result = await _categoryManager.Delete(id);

            var message = result.Type == ResultType.Success
                ? "Category has been deleted"
                : result.BuildMessage();

            var title = "Completed";

            if (result.Type == ResultType.Error)
            {
                title = "Attention";
            }
            else if (result.Type == ResultType.Warning)
            {
                title = "Oops";
            }

            return Json(new
            {
                title,
                message,
                MessageType = result.Type.ToString().ToLower()
            });
        }

        [HttpGet]
        public async Task<JsonResult> NameIsUnique(string name)
        {
            bool categoryNotExists = await _categoryManager.CategoryNotExists(name);

            return Json(categoryNotExists);
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
                _categoryManager.Dispose();
            }

            base.Dispose(disposing);
            _disposedValue = true;
        }

        #endregion
    }
}