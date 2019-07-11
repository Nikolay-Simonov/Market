using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Market.Models.AdminViewModels
{
    public class EmployeeCreateVM
    {
        public string Id { get; set; }

        /// <summary>
        /// Имя.
        /// </summary>
        [DisplayName("First Name"), Required]
        [StringLength(25, ErrorMessage = StringLengthError, MinimumLength = 2)]
        [RegularExpression(@"[A-zА-яЙйёЁ]+", ErrorMessage = CyrLatRegexError)]
        public string FirstName { get; set; }

        /// <summary>
        /// Отчество.
        /// </summary>
        [DisplayName("Middle Name")]
        [StringLength(25, ErrorMessage = StringLengthError, MinimumLength = 0)]
        [RegularExpression(@"[A-zА-яЙйёЁ]+", ErrorMessage = CyrLatRegexError)]
        public string MiddleName { get; set; }

        /// <summary>
        /// Фамилия.
        /// </summary>
        [DisplayName("Last Name"), Required]
        [StringLength(25, ErrorMessage = StringLengthError, MinimumLength = 2)]
        [RegularExpression(@"[A-zА-яЙйёЁ]+", ErrorMessage = CyrLatRegexError)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        /// <summary>
        /// Роли сотрудника.
        /// </summary>
        public HashSet<string> Roles { get; set; }

        /// <summary>
        /// Доступные роли.
        /// </summary>
        public HashSet<string> AvailableRoles { get; set; }

        private const string StringLengthError =
            "The {0} must be at least {2} and at max {1} characters long.";

        private const string CyrLatRegexError =
            "The {0} can contain only Cyrillic and Latin characters.";
    }
}