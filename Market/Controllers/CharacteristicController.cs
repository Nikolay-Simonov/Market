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
    public class CharacteristicController : Controller
    {
        private readonly ICharacteristicManager _characteristicManager;
        //private readonly IToastNotification _toast;

        public CharacteristicController(ICharacteristicManager characteristicManager)//, IToastNotification toast)
        {
            _characteristicManager = characteristicManager;
            //_toast = toast;
        }

        [HttpGet]
        public async Task<ActionResult> List(string name)
        {
            IEnumerable<CharacteristicVM> characteristics =
                (await _characteristicManager.Characteristics(name)).Select(c => new CharacteristicVM
                {
                    Id = c.Id,
                    Name = c.Name
                });

            return View(characteristics);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(CharacteristicVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            OperationResult result = await _characteristicManager.CreateAsync(model.Name);

            if (result.Type == ResultType.Success)
            {
                //_toast.AddSuccessToastMessage("Characteristic has been successfully created");
                //return View();
                return RedirectToAction(nameof(List));
            }

            ModelState.AddModelError(nameof(model.Name), result.BuildMessage());

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {
            CharacteristicDTO characteristic = await _characteristicManager.GetAsync(id);

            if (characteristic == null)
            {
                return NotFound("Characteristic doesn't exist");
            }

            return View(new CharacteristicVM
            {
                Id = characteristic.Id,
                Name = characteristic.Name
            });
        }

        [HttpPost]
        public async Task<ActionResult> Edit(CharacteristicVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            OperationResult result = await _characteristicManager.Edit(new CharacteristicDTO
            {
                Id = model.Id,
                Name = model.Name
            });

            if (result.Type == ResultType.Success)
            {
                // _toast.AddSuccessToastMessage("Characteristic has been successfully edited");
                // return View(model);
                return RedirectToAction(nameof(List));
            }

            ModelState.AddModelError(nameof(model.Name), result.BuildMessage());

            return View(model);
        }

        [HttpPost]
        public async Task<JsonResult> Delete(int id)
        {
            OperationResult result = await _characteristicManager.Delete(id);

            var message = result.Type == ResultType.Success
                ? "Characteristic has been deleted"
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
            bool characteristicNotExists = await _characteristicManager.CharacteristicNotExists(name);

            return Json(characteristicNotExists);
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
                _characteristicManager.Dispose();
            }

            base.Dispose(disposing);
            _disposedValue = true;
        }

        #endregion
    }
}