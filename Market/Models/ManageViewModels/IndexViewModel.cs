using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Market.Models.ManageViewModels
{
    public class IndexViewModel
    {
        private const string StringLengthError =
            "The {0} must be at least {2} and at max {1} characters long.";

        public string Username { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Адрес доставки
        /// </summary>
        [DisplayName("Shipping adress"), Required]
        [StringLength(150, ErrorMessage = StringLengthError, MinimumLength = 5)]
        public string Address { get; set; }

        public string StatusMessage { get; set; }
    }
}
