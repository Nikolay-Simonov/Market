namespace Market.BLL.DTO
{
    public class CatalogSearchDTO
    {
        public string NameOrDescription { get; set; }

        public int PageSize { get; set; }

        public uint Page { get; set; }
    }
}