using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Market.Models.ProductsManagmentViewModels
{
    public class ProductsListVM
    {
        public PagingInfoVM PagingInfo { get; set; }

        public ProductsFiltersVM Filters { get; set; }

        public SelectList Brands { get; set; }

        public SelectList Countries { get; set; }

        public SelectList Categories { get; set; }

        public IEnumerable<ProductDataVM> Products { get; set; }
    }
}