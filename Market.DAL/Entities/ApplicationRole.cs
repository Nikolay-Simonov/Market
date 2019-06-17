using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Market.DAL.Entities
{
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole() : base()
        {
        }

        public ApplicationRole(string roleName) : base(roleName)
        {
        }

        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
    }
}