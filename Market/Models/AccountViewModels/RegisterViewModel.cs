using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Market.Models.AccountViewModels
{
    public class RegisterViewModel
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
        [DisplayName("Shipping address"), Required]
        [StringLength(150, ErrorMessage = StringLengthError, MinimumLength = 5)]
        public string Address { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
