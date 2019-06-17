using System.ComponentModel.DataAnnotations;
using Market.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Market.Models
{
    public class CountryVM
    {
        public int Id { get; set; }

        [Required, Remote(nameof(CountryController.NameIsUnique), "Country",
             ErrorMessage = "Country already exists")]
        [StringLength(120, ErrorMessage = StringLengthError, MinimumLength = 2)]
        [RegularExpression(@"[A-zА-яёЁ ]+", ErrorMessage = CyrLatRegexError)]
        public string Name
        {
            get => _name;
            set => _name = value?.Trim();
        }

        private const string StringLengthError =
            "The {0} must be at least {2} and at max {1} characters long.";

        private const string CyrLatRegexError =
            @"The {0} can contain only Cyrillic, Latin and space characters";

        private string _name;
    }
}