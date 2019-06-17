using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Market.Models
{
    public class ApplicationRoleVM : IdentityRole
    {
        public ApplicationRoleVM() : base()
        {
        }

        public ApplicationRoleVM(string roleName) : base(roleName)
        {
        }

        public List<ApplicationUserRoleVM> UserRoles { get; set; }
    }
}