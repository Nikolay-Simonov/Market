using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Market.BLL.DTO;
using Market.BLL.Extensions;
using Market.BLL.Interfaces;
using Market.Models;
using Market.Models.AdminViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Market.Controllers
{
    [AutoValidateAntiforgeryToken, Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IStaffManager _staffManager;

        public AdminController(IStaffManager staffManager)
        {
            _staffManager = staffManager;
        }

        public int PageSize { get; set; } = 15;

        [HttpGet]
        public async Task<ActionResult> Staff(StaffListVM model, uint page)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            StaffListDTO staffList = await _staffManager.Staff(new StaffFiltersDTO
            {
                NameOrEmail = model.NameOrEmail,
                Page = page,
                PageSize = PageSize
            });

            model.Staff = staffList.Staff.Select(user => new StaffDataVM
            {
                Id = user.Id,
                Email = user.Email,
                LastName = user.LastName,
                FirtsName = user.FirstName,
                MiddelName = user.MiddleName,
                PhoneNumber = user.PhoneNumber
            });
            model.PagingInfo = new PagingInfoVM
            {
                CurrentPage = staffList.PagingInfo.CurrentPage,
                TotalItems = staffList.PagingInfo.TotalItems,
                ItemsCurrentPage = staffList.PagingInfo.ItemsCurrentPage,
                ItemsPerPage = staffList.PagingInfo.ItemsPerPage
            };

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> CreateEmployee()
        {
            var viewModel = new EmployeeCreateVM
            {
                AvailableRoles = new SortedSet<string>(await _staffManager.StaffRoles())
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> CreateEmployee(EmployeeCreateVM model)
        {
            model.AvailableRoles = new SortedSet<string>(await _staffManager.StaffRoles());

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            IdentityResult result = await _staffManager.CreateAsync(new EmployeeCreateDTO
            {
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                MiddleName = model.MiddleName,
                Roles = model.Roles
            });

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Staff));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> EditEmployee(string id)
        {
            ApplicationUserDTO user = await _staffManager.GetAsync(id);

            if (user == null || user.Id != id)
            {
                return NotFound();
            }

            var viewModel = new EmployeeCreateVM
            {
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                LastName = user.LastName,
                Email = user.Email,
                Roles = new SortedSet<string>(user.ApplicationRoles.Select(r => r.Name)),
                AvailableRoles = new SortedSet<string>(await _staffManager.StaffRoles())
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> EditEmployee(EmployeeCreateVM editModel)
        {
            editModel.AvailableRoles = new SortedSet<string>(await _staffManager.StaffRoles());

            if (!ModelState.IsValid)
            {
                return View(editModel);
            }

            IdentityResult result = await _staffManager.EditAsync(new EmployeeCreateDTO
            {
                Id = editModel.Id,
                Email = editModel.Email,
                FirstName = editModel.FirstName,
                LastName = editModel.LastName,
                MiddleName = editModel.MiddleName,
                Roles = editModel.Roles
            });

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Staff));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(editModel);
        }

        [HttpPost]
        public async Task<JsonResult> Delete(string id)
        {
            IdentityResult deleteResult = await _staffManager.DeleteAsync(id);
            var message = deleteResult.Succeeded
                ? "The employee account has been successfully deleted."
                : deleteResult.BuildMessage();
            var title = deleteResult.Succeeded ? "Completed" : "Attention";
            var messageType = deleteResult.Succeeded ? "success" : "error";

            return Json(new { message, title, messageType });
        }

        [HttpPost]
        public async Task<JsonResult> ResetPassword(string id, string password)
        {
            IdentityResult resetResult = await _staffManager
                .ResetPasswordWithSendOnEmail(id, password);
            var message = resetResult.Succeeded
                ? "The password was successfully changed and sent to the mail."
                : resetResult.BuildMessage();
            var title = resetResult.Succeeded ? "Completed" : "Attention";
            var messageType = resetResult.Succeeded ? "success" : "error";

            return Json(new { message, title, messageType });
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
                _staffManager.Dispose();
            }

            base.Dispose(disposing);
            _disposedValue = true;
        }

        #endregion
    }
}