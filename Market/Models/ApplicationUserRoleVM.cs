using Microsoft.AspNetCore.Identity;

namespace Market.Models
{
    public class ApplicationUserRoleVM : IdentityUserRole<string>
    {
        public ApplicationUserVM User { get; set; }

        public ApplicationRoleVM Role { get; set; }
    }
}