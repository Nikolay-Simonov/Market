using System.ComponentModel.DataAnnotations;
using Market.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Market.Models
{
    public class CharacteristicVM
    {
        public int Id { get; set; }

        [Required, Remote(nameof(CharacteristicController.NameIsUnique),
             "Characteristic", ErrorMessage = "Characteristic already exists")]
        [StringLength(60, ErrorMessage = StringLengthError, MinimumLength = 2)]
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