using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Market.BLL.DTO;
using Microsoft.AspNetCore.Identity;

namespace Market.BLL.Interfaces
{
    public interface IStaffManager : IDisposable
    {
        Task<StaffListDTO> Staff(StaffFiltersDTO staffFilters);

        Task<IdentityResult> CreateAsync(EmployeeCreateDTO createModel);

        Task<IdentityResult> EditAsync(EmployeeCreateDTO editModel);

        Task<IdentityResult> DeleteAsync(string id);

        Task<IdentityResult> ResetPasswordWithSendOnEmail(string id, string password = null);

        Task<IEnumerable<string>> StaffRoles();

        Task<ApplicationUserDTO> GetAsync(string id);
    }
}