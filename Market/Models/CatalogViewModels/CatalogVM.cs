using System.Collections.Generic;
using System.ComponentModel;
using Market.BLL.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Market.Models.CatalogViewModels
{
    public class CatalogVM
    {
        public PagingInfoVM PagingInfo { get; set; }

        public decimal MaxPrice
        {
            get => _maxPrice;
            set => _maxPrice = value < 0 ? 0 : value;
        }

        public decimal MinPrice
        {
            get => _minPrice;
            set => _minPrice = value < 0 ? 0 : value;
        }

        public double MaxWeight
        {
            get => _maxWeight;
            set => _maxWeight = value < 0 ? 0 : value;
        }

        public double MinWeight
        {
            get => _minWeight;
            set => _minWeight = value < 0 ? 0 : value;
        }

        [DisplayName("Start price")]
        public decimal? StartPrice
        {
            get => _startPrice;
            set => _startPrice = value != null && value < 0 ? 0 : value;
        }

        [DisplayName("Start weight")]
        public double? StartWeight
        {
            get => _startWeight;
            set => _startWeight = value != null && value < 0 ? 0 : value;
        }

        [DisplayName("End price")]
        public decimal? EndPrice
        {
            get => _endPrice;
            set => _endPrice = value != null && value < 0 ? 0 : value;
        }

        [DisplayName("End weight")]
        public double? EndWeight
        {
            get => _endWeight;
            set => _endWeight = value != null && value < 0 ? 0 : value;
        }

        public HashSet<FacetCriterionVM> FacetsCriteries { get; set; }

        public HashSet<string> Countries { get; set; }

        public HashSet<string> Brands { get; set; }

        public Dictionary<string, HashSet<string>> Characteristics { get; set; }

        public string Category { get; set; }

        [DisplayName("Sort by")]
        public CatalogSortField SortField { get; set; } = CatalogSortField.Price;

        [DisplayName("Sort direction")]
        public SortingDirection SortingDirection { get; set; } = SortingDirection.Ascending;

        public IEnumerable<ProductShortVM> Products { get; set; }

        private decimal _maxPrice;
        private decimal _minPrice;
        private double _maxWeight;
        private double _minWeight;

        private decimal? _endPrice;
        private decimal? _startPrice;
        private double? _endWeight;
        private double? _startWeight;
    }
}