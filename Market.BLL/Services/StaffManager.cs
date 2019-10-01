using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Market.BLL.DTO;
using Market.BLL.Interfaces;
using Market.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Market.BLL.Services
{
    internal class StaffManager : IStaffManager
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IEmailSender _emailSender;
        private readonly IPasswordGenerator _pwdGenerator;

        public StaffManager(UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager, IEmailSender emailSender,
            IPasswordGenerator pwdGenerator)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _emailSender = emailSender;
            _pwdGenerator = pwdGenerator;
        }

        /// <summary>
        /// Возвращает список персонала отфильтрованный по <paramref name="filters"/>.
        /// </summary>
        public async Task<StaffListDTO> Staff(StaffFiltersDTO filters)
        {
            IQueryable<ApplicationUser> users = _userManager.Users.Where(u =>
                u.UserRoles.All(r => r.Role.Name != "User" && r.Role.Name != "Admin")
            );

            if (filters == null)
            {
                return new StaffListDTO
                {
                    PagingInfo = new PagingInfoDTO
                    {
                        CurrentPage = 1,
                        ItemsPerPage = 15,
                        TotalItems = await users.CountAsync()
                    },
                    // Скипаем ноль объектов, и берем 15.
                    Staff = await MapToDto(0, 15)
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

            if (!string.IsNullOrEmpty(filters.NameOrEmail))
            {
                users = users.Where(u =>
                    u.Email.Contains(filters.NameOrEmail)
                    || (u.FirstName + " " + (string.IsNullOrWhiteSpace(u.MiddleName) ? "" : u.MiddleName + " ")
                        + u.LastName).Contains(filters.NameOrEmail)
                );
            }

            int totalItems = await users.CountAsync();
            int skipCount = (int)(filters.Page - 1) * filters.PageSize;

            var staffListDto = new StaffListDTO
            {
                Staff = await MapToDto(skipCount, filters.PageSize),
                PagingInfo = new PagingInfoDTO
                {
                    CurrentPage = (int)filters.Page,
                    ItemsPerPage = filters.PageSize,
                    TotalItems = totalItems
                }
            };

            return staffListDto;

            #region Local Functions

            async Task<IEnumerable<ApplicationUserDTO>> MapToDto(int skip, int pageSize)
            {
                var dtoList = await users.Select(p => new ApplicationUserDTO
                {
                    Id = p.Id,
                    AccessFailedCount = p.AccessFailedCount,
                    Address = p.Address,
                    ApplicationRoles = p.UserRoles.Select(ur => new ApplicationRoleDTO
                    {
                        Id = ur.Role.Id,
                        ConcurrencyStamp = ur.Role.ConcurrencyStamp,
                        Name = ur.Role.Name,
                        NormalizedName = ur.Role.NormalizedName
                    }).ToList(),
                    ConcurrencyStamp = p.ConcurrencyStamp,
                    UserName = p.UserName,
                    Email = p.Email,
                    LastName = p.LastName,
                    MiddleName = p.MiddleName,
                    FirstName = p.FirstName,
                    EmailConfirmed = p.EmailConfirmed,
                    LockoutEnabled = p.LockoutEnabled,
                    LockoutEnd = p.LockoutEnd,
                    NormalizedEmail = p.NormalizedEmail,
                    NormalizedUserName = p.NormalizedUserName,
                    PasswordHash = p.PasswordHash,
                    PhoneNumber = p.PhoneNumber,
                    PhoneNumberConfirmed = p.PhoneNumberConfirmed,
                    SecurityStamp = p.SecurityStamp,
                    TwoFactorEnabled = p.TwoFactorEnabled
                }).OrderBy(p => p.Id).Skip(skip).Take(pageSize).ToArrayAsync();

                return dtoList;
            }

            #endregion
        }

        public async Task<IEnumerable<string>> StaffRoles()
        {
            return await _roleManager.Roles
                .Where(r => r.Name != "Admin" && r.Name != "User")
                .Select(r => r.Name)
                .ToArrayAsync();
        }

        public async Task<ApplicationUserDTO> GetAsync(string id)
        {
            ApplicationUser user = await _userManager.Users
                .Include(ur => ur.UserRoles)
                .ThenInclude(r => r.Role)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null || user.Id != id)
            {
                return null;
            }

            var dto = new ApplicationUserDTO
            {
                Id = user.Id,
                AccessFailedCount = user.AccessFailedCount,
                Address = user.Address,
                ApplicationRoles = user.UserRoles.Select(ur => new ApplicationRoleDTO
                {
                    Id = ur.Role.Id,
                    ConcurrencyStamp = ur.Role.ConcurrencyStamp,
                    Name = ur.Role.Name,
                    NormalizedName = ur.Role.NormalizedName
                }).ToList(),
                ConcurrencyStamp = user.ConcurrencyStamp,
                UserName = user.UserName,
                Email = user.Email,
                LastName = user.LastName,
                MiddleName = user.MiddleName,
                FirstName = user.FirstName,
                EmailConfirmed = user.EmailConfirmed,
                LockoutEnabled = user.LockoutEnabled,
                LockoutEnd = user.LockoutEnd,
                NormalizedEmail = user.NormalizedEmail,
                NormalizedUserName = user.NormalizedUserName,
                PasswordHash = user.PasswordHash,
                PhoneNumber = user.PhoneNumber,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                SecurityStamp = user.SecurityStamp,
                TwoFactorEnabled = user.TwoFactorEnabled
            };

            return dto;
        }

        public async Task<IdentityResult> CreateAsync(EmployeeCreateDTO createModel)
        {
            if (createModel == null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = string.Empty,
                    Description = "Please fill in the required fields"
                });
            }

            var user = new ApplicationUser
            {
                UserName = createModel.Email,
                Email = createModel.Email,
                Address = "",
                FirstName = createModel.FirstName,
                MiddleName = createModel.MiddleName,
                LastName = createModel.LastName,
                EmailConfirmed = true
            };

            IEnumerable<string> staffRoles = await StaffRoles();
            List<string> availableRoles = createModel.Roles?.Where(r =>
                staffRoles.Contains(r, StringComparer.OrdinalIgnoreCase)
            ).ToList();

            string randomPwd = _pwdGenerator.GetNext();
            IdentityResult result = await _userManager.CreateAsync(user, randomPwd);

            if (!result.Succeeded)
            {
                return IdentityResult.Failed(result.Errors.ToArray());
            }

            await _emailSender.SendEmailAsync(user.Email, "Register at the market",
                "Your employee account password: " + randomPwd);

            if (availableRoles == null || !availableRoles.Any())
            {
                return result;
            }

            IdentityResult rolesResult = await _userManager
                .AddToRolesAsync(user, availableRoles);

            if (rolesResult.Succeeded)
            {
                return result;
            }

            List<IdentityError> errors = result.Errors.ToList();
            errors.AddRange(rolesResult.Errors);
            result = IdentityResult.Failed(errors.ToArray());

            return result;
        }

        public async Task<IdentityResult> EditAsync(EmployeeCreateDTO editModel)
        {
            var notFound = new IdentityError
            {
                Code = string.Empty,
                Description = "No employee found."
            };

            if (editModel == null)
            {
                return IdentityResult.Failed(notFound);
            }

            ApplicationUser user = await _userManager.FindByIdAsync(editModel.Id);

            if (user == null || user.Id != editModel.Id)
            {
                return IdentityResult.Failed(notFound);
            }

            user.Id = editModel.Id;
            user.UserName = editModel.Email;
            user.Email = editModel.Email;
            user.FirstName = editModel.FirstName;
            user.MiddleName = editModel.MiddleName;
            user.LastName = editModel.LastName;

            IEnumerable<string> staffRoles = await StaffRoles();
            List<string> availableRoles = editModel.Roles?.Where(r =>
                staffRoles.Contains(r, StringComparer.OrdinalIgnoreCase)
            ).ToList();
            List<IdentityError> errors = new List<IdentityError>();
            IdentityResult updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                return IdentityResult.Failed(updateResult.Errors.ToArray());
            }

            IdentityResult resetPwdResult = await ResetPasswordWithSendOnEmail(user);

            if (!resetPwdResult.Succeeded)
            {
                errors.AddRange(resetPwdResult.Errors);
            }

            IList<string> currentUserRoles = await _userManager.GetRolesAsync(user);
            IdentityResult rolesRemoveResult = await _userManager
                .RemoveFromRolesAsync(user, currentUserRoles);

            if (!rolesRemoveResult.Succeeded)
            {
                errors.AddRange(rolesRemoveResult.Errors);
            }
            else if (availableRoles != null && availableRoles.Any())
            {
                IdentityResult rolesAddResult = await _userManager
                    .AddToRolesAsync(user, availableRoles);

                if (!rolesAddResult.Succeeded)
                {
                    errors.AddRange(rolesAddResult.Errors);
                }
            }

            return errors.Any()
                ? IdentityResult.Failed(errors.ToArray())
                : updateResult;
        }

        public async Task<IdentityResult> DeleteAsync(string id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);

            if (user == null || user.Id != id)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = string.Empty,
                    Description = "Employee is not found."
                });
            }

            // Существует возможность подмены Id, а пользователей удалять нельзя.
            if (await _userManager.IsInRoleAsync(user, "User"))
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = string.Empty,
                    Description = "This account is also a user account, so it cannot be deleted."
                });
            }

            return await _userManager.DeleteAsync(user);
        }

        public async Task<IdentityResult> ResetPasswordWithSendOnEmail(string id, string password = null)
        {
            return await ResetPasswordWithSendOnEmail(await _userManager.FindByIdAsync(id), password);
        }

        private async Task<IdentityResult> ResetPasswordWithSendOnEmail(ApplicationUser user,
            string password = null)
        {
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = string.Empty,
                    Description = "User is not found"
                });
            }

            bool pwdIsValid = false;

            if (!string.IsNullOrWhiteSpace(password))
            {
                // Необходимо проверить валидность пароля для текущих правил.
                Task<IdentityResult>[] validatorsTasks = _userManager.PasswordValidators
                    .Select(v => v.ValidateAsync(_userManager, user, password))
                    .ToArray();
                pwdIsValid = await Task.Factory.ContinueWhenAll(validatorsTasks,
                    tasks => tasks.All(t => t.Result.Succeeded)
                );
            }

            bool userHasPwd = await _userManager.HasPasswordAsync(user);
            string tokenResult;

            if (pwdIsValid)
            {
                // Если пароль валидный и у пользователя не было пароля,
                // добавляем пароль.
                if (!userHasPwd)
                {
                    IdentityResult addPwdResult = await _userManager
                        .AddPasswordAsync(user, password);
                    await SendPassword(addPwdResult, user.Email, password);

                    return addPwdResult;
                }

                // Иначе, проверям действующий
                // ли это пароль и генериуем новый, если это так.
                IdentityError error = null;

                while (await _userManager.CheckPasswordAsync(user, password))
                {
                    if (error == null)
                    {
                        error = new IdentityError
                        {
                            Code = string.Empty,
                            Description = "The current password was specified, so the "
                                + "system generated a new password."
                        };
                    }

                    password = _pwdGenerator.GetNext();
                }

                tokenResult = await _userManager.GeneratePasswordResetTokenAsync(user);
                IdentityResult resetPwdResult =  await _userManager
                    .ResetPasswordAsync(user, tokenResult, password);
                await SendPassword(resetPwdResult, user.Email, password);

                return error == null ? resetPwdResult : IdentityResult.Failed(error);
            }

            // Если пароль невалиден и у юзера не было пароля,
            // то просто генерируем новый.
            if (!userHasPwd)
            {
                password = _pwdGenerator.GetNext();
                IdentityResult addPwdResult = await _userManager
                    .AddPasswordAsync(user, password);
                await SendPassword(addPwdResult, user.Email, password);

                return addPwdResult;
            }

            // Если пароль невалиден и у юзера есть пароль, то проверям действующий
            // ли это пароль и генериуем новый, если это так.
            do
            {
                password = _pwdGenerator.GetNext();
            } while (await _userManager.CheckPasswordAsync(user, password));

            tokenResult = await _userManager.GeneratePasswordResetTokenAsync(user);
            IdentityResult resetPasswordResult = await _userManager
                .ResetPasswordAsync(user, tokenResult, password);
            await SendPassword(resetPasswordResult, user.Email, password);

            return resetPasswordResult;

            #region Local Functions

            async Task SendPassword(IdentityResult identityResult, string email, string pwd)
            {
                if (identityResult.Succeeded)
                {
                    await _emailSender.SendEmailAsync(email, "Account Changes",
                        "Your employee account password: " + pwd);
                }
            }

            #endregion
        }

        #region IDisposable Support

        private bool _disposedValue;

        private void Dispose(bool disposing)
        {
            if (_disposedValue)
            {
                return;
            }

            if (disposing)
            {
                _userManager.Dispose();
                _roleManager.Dispose();
            }

            _disposedValue = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}