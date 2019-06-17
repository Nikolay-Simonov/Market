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
    public class CountryController : Controller
    {
        private readonly ICountryManager _countryManager;
        // private readonly IToastNotification _toast;

        public CountryController(ICountryManager countryManager)//, IToastNotification toast)
        {
            _countryManager = countryManager;
            //_toast = toast;
        }

        [HttpGet]
        public async Task<ActionResult> List(string name)
        {
            IEnumerable<CountryVM> countries = (await _countryManager.Countries(name)).Select(c => new CountryVM
            {
                Id = c.Id,
                Name = c.Name
            });

            return View(countries);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(CountryVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            OperationResult result = await _countryManager.CreateAsync(model.Name);

            if (result.Type == ResultType.Success)
            {
                //_toast.AddSuccessToastMessage("Country has been successfully created");
                //return View();
                return RedirectToAction(nameof(List));
            }

            ModelState.AddModelError(nameof(model.Name), result.BuildMessage());

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {
            CountryDTO country = await _countryManager.GetAsync(id);

            if (country == null)
            {
                return NotFound("Country doesn't exist");
            }

            return View(new CountryVM
            {
                Id = country.Id,
                Name = country.Name
            });
        }

        [HttpPost]
        public async Task<ActionResult> Edit(CountryVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            OperationResult result = await _countryManager.Edit(new CountryDTO
            {
                Id = model.Id,
                Name = model.Name
            });

            if (result.Type == ResultType.Success)
            {
                //_toast.AddSuccessToastMessage("Country has been successfully edited");
                //return View(model);
                return RedirectToAction(nameof(List));
            }

            ModelState.AddModelError(nameof(model.Name), result.BuildMessage());

            return View(model);
        }

        [HttpPost]
        public async Task<JsonResult> Delete(int id)
        {
            OperationResult result = await _countryManager.Delete(id);

            var message = result.Type == ResultType.Success
                ? "Country has been deleted"
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
            bool countryNotExists = await _countryManager.CountryNotExists(name);

            return Json(countryNotExists);
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
                _countryManager.Dispose();
            }

            base.Dispose(disposing);
            _disposedValue = true;
        }

        #endregion
    }
}