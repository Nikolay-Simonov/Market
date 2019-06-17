using Market.DAL.Enums;

namespace Market.BLL.DTO
{
    public class ProductsFiltersDTO
    {
        public decimal? Price { get; set; }

        public double? Weight { get; set; }

        public string Name { get; set; }

        public string Country { get; set; }

        public string Category { get; set; }

        public string Brand { get; set; }

        public RemoveState Removed { get; set; } = RemoveState.No;

        public uint Page { get; set; }

        public int PageSize { get; set; }
    }
}