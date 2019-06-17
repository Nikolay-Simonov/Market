using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Market.Models
{
    public class ApplicationUserVM : IdentityUser
    {
        private const string StringLengthError =
            "The {0} must be at least {2} and at max {1} characters long.";

        private const string CyrLatRegexError =
            "The {0} can contain only Cyrillic and Latin characters.";

        /// <summary>
        /// Имя
        /// </summary>
        [DisplayName("First Name"), Required]
        [StringLength(25, ErrorMessage = StringLengthError, MinimumLength = 2)]
        [RegularExpression(@"[A-zА-яЙйёЁ]+", ErrorMessage = CyrLatRegexError)]
        public string FirstName { get; set; }

        /// <summary>
        /// Отчество
        /// </summary>
        [DisplayName("Middle Name")]
        [StringLength(25, ErrorMessage = StringLengthError, MinimumLength = 0)]
        [RegularExpression(@"[A-zА-яЙйёЁ]+", ErrorMessage = CyrLatRegexError)]
        public string MiddleName { get; set; }

        /// <summary>
        /// Фамилия
        /// </summary>
        [DisplayName("Last Name"), Required]
        [StringLength(25, ErrorMessage = StringLengthError, MinimumLength = 2)]
        [RegularExpression(@"[A-zА-яЙйёЁ]+", ErrorMessage = CyrLatRegexError)]
        public string LastName { get; set; }

        /// <summary>
        /// Адрес доставки
        /// </summary>
        [DisplayName("Shipping adress"), Required]
        [StringLength(150, ErrorMessage = StringLengthError, MinimumLength = 5)]
        public string Address { get; set; }

        /// <summary>
        /// Полное имя
        /// </summary>
        [NotMapped]
        public string FullName => string.IsNullOrWhiteSpace(MiddleName)
            ? FirstName + " " + LastName
            : FirstName + " " + MiddleName + " " + LastName;

        public List<ApplicationUserRoleVM> UserRoles { get; set; }
    }
}