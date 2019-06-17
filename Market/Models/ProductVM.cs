using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Market.Models
{
    public class ProductVM
    {
        public int Id { get; set; }

        public int? CategoryId { get; set; }
        public CategoryVM Category { get; set; }

        public int? BrandId { get; set; }
        public BrandVM Brand { get; set; }

        public int? CountryId { get; set; }
        public CountryVM Country { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = StringLengthError, MinimumLength = 2)]
        [RegularExpression(NameRegex, ErrorMessage = NameRegexError)]
        public string Name
        {
            get => _name;
            set => _name = value?.Trim();
        }

        [Required]
        [StringLength(1000, ErrorMessage = StringLengthError, MinimumLength = 10)]
        [RegularExpression(DescRegex, ErrorMessage = DescRegexError)]
        public string Description
        {
            get => _description;
            set => _description = value?.Trim();
        }

        /// <summary>
        /// Вес в граммах.
        /// </summary>
        [Range(0.0D, MaxWeightGrams), Required]
        public double Weight
        {
            get => _weight;
            set => _weight = value < 0 ? 0 : value;
        }

        [Range(0.0D, MaxPrice), Required]
        public decimal Price
        {
            get => _price;
            set => _price = value < 0 ? 0 : value;
        }

        public string Image { get; set; }

        [DisplayName("Characteristics")]
        public string Character { get; set; }

        public bool Removed { get; set; }

        private const int MaxWeightGrams = 50000;
        private const int MaxPrice = 200000;
        private const string StringLengthError =
            "The {0} must be at least {2} and at max {1} characters long.";
        private const string NameRegexError =
            "The {0} can contain Latin, Cyrillic, space, dot, comma and dash characters.";
        private const string DescRegexError =
            "The {0} can contain punctuation, currency, digit, Latin, and Cyrillic characters.";

        private const string NameRegex = @"[\w\dА-яёЁ., —-]+";
        private const string DescRegex = @"[\d\w\sА-яёЁ""@№$;:%€?&().,'—-]+";

        private decimal _price;
        private double _weight;
        private string _description;
        private string _name;
    }
}