namespace Market.BLL.DTO
{
    public class StaffFiltersDTO
    {
        public string NameOrEmail { get; set; }

        public uint Page { get; set; }

        public int PageSize { get; set; }
    }
}