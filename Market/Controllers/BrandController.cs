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
    public class BrandController : Controller
    {
        private readonly IBrandManager _brandManager;
        //private readonly IToastNotification _toast;

        public BrandController(IBrandManager brandManager)//, IToastNotification toast)
        {
            _brandManager = brandManager;
            //_toast = toast;
        }

        [HttpGet]
        public async Task<ActionResult> List(string name)
        {
            IEnumerable<BrandVM> brands = (await _brandManager.Brands(name)).Select(b => new BrandVM
            {
                Id = b.Id,
                Name = b.Name
            });

            return View(brands);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(BrandVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            OperationResult result = await _brandManager.CreateAsync(model.Name);

            if (result.Type == ResultType.Success)
            {
                //_toast.AddSuccessToastMessage("Brand has been successfully created");
                //return View();
                return RedirectToAction(nameof(List));
            }

            ModelState.AddModelError(nameof(model.Name), result.BuildMessage());

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {
            BrandDTO brand = await _brandManager.GetAsync(id);

            if (brand == null)
            {
                return NotFound("Brand doesn't exist");
            }

            return View(new BrandVM
            {
                Id = brand.Id,
                Name = brand.Name
            });
        }

        [HttpPost]
        public async Task<ActionResult> Edit(BrandVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            OperationResult result = await _brandManager.Edit(new BrandDTO
            {
                Id = model.Id,
                Name = model.Name
            });

            if (result.Type == ResultType.Success)
            {
                //_toast.AddSuccessToastMessage("Brand has been successfully edited");
                //return View(model);
                return RedirectToAction(nameof(List));
            }

            ModelState.AddModelError(nameof(model.Name), result.BuildMessage());

            return View(model);
        }

        [HttpPost]
        public async Task<JsonResult> Delete(int id)
        {
            OperationResult result = await _brandManager.Delete(id);

            var message = result.Type == ResultType.Success
                ? "Brand has been deleted"
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
            bool brandNotExists = await _brandManager.BrandNotExists(name);

            return Json(brandNotExists);
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
                _brandManager.Dispose();
            }

            base.Dispose(disposing);
            _disposedValue = true;
        }

        #endregion
    }
}