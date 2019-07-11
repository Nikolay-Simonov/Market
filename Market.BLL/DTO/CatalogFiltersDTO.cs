using System.Collections.Generic;
using Market.BLL.Enums;

namespace Market.BLL.DTO
{
    public class CatalogFiltersDTO
    {
        public decimal? StartPrices { get; set; }

        public decimal? EndPrices { get; set; }

        public double? StartWeight { get; set; }

        public double? EndWeight { get; set; }

        public HashSet<string> Countries { get; set; }

        public HashSet<string> Brands { get; set; }

        public string Category { get; set; }

        public Dictionary<string, HashSet<string>> Characteristics { get; set; }

        public CatalogSortField SortField { get; set; } = CatalogSortField.Price;

        public SortingDirection SortingDirection { get; set; } = SortingDirection.Ascending;

        public uint Page { get; set; }

        public int PageSize { get; set; }
    }
}