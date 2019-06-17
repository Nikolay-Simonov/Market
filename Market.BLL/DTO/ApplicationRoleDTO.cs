using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Identity;

namespace Market.BLL.DTO
{
    [DataContract(IsReference = true)]
    public class ApplicationRoleDTO : IdentityRole
    {
        public ApplicationRoleDTO() : base()
        {
        }

        public ApplicationRoleDTO(string roleName) : base(roleName)
        {
        }
        
        [DataMember]
        public List<ApplicationUserDTO> ApplicationUsers { get; set; }
    }
}