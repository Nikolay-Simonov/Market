using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Market.DAL.Entities
{
    public class Product
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int? CategoryId { get; set; }
        public virtual Category Category { get; set; }

        public int? BrandId { get; set; }
        public virtual Brand Brand { get; set; }

        public int? CountryId { get; set; }
        public virtual Country Country { get; set; }

        [Required]
        [StringLength(100)]
        public string Name
        {
            get => _name;
            set => _name = value?.Trim();
        }

        [Required]
        [StringLength(1000)]
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

        public string Character { get; set; }

        public bool Removed { get; set; }

        private const int MaxWeightGrams = 50000;
        private const int MaxPrice = 200000;

        private decimal _price;
        private double _weight;
        private string _description;
        private string _name;
    }
}