using System.Collections.Generic;

namespace Market.BLL.DTO
{
    public class ProductsListDTO
    {
        public IEnumerable<ProductDTO> Products { get; set; }

        public PagingInfoDTO PagingInfo { get; set; }
    }
}