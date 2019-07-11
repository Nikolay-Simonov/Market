using System.Collections.Generic;

namespace Market.Models.CatalogViewModels
{
    public class SearchResultsVM
    {
        public IEnumerable<ProductShortVM> Products { get; set; }

        public PagingInfoVM PagingInfo { get; set; }
    }
}