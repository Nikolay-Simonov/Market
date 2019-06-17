using System.Runtime.Serialization;
using Microsoft.AspNetCore.Identity;

namespace Market.BLL.DTO
{
    [DataContract(IsReference = true)]
    public class ApplicationUserRoleDTO : IdentityUserRole<string>
    {
        [DataMember]
        public ApplicationUserDTO User { get; set; }

        [DataMember]
        public ApplicationRoleDTO Role { get; set; }
    }
}