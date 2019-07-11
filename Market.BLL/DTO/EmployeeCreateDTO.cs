using System.Collections.Generic;

namespace Market.BLL.DTO
{
    public class EmployeeCreateDTO
    {
        public string Id { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public HashSet<string> Roles { get; set; }
    }
}