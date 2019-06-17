using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Identity;

namespace Market.BLL.DTO
{
    [DataContract(IsReference = true)]
    public class ApplicationUserDTO : IdentityUser
    {
        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string MiddleName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public string Address { get; set; }

        [DataMember]
        public string FullName => string.IsNullOrWhiteSpace(MiddleName)
            ? FirstName + " " + LastName
            : FirstName + " " + MiddleName + " " + LastName;

        [DataMember]
        public List<ApplicationRoleDTO> ApplicationRoles { get; set; }
    }
}