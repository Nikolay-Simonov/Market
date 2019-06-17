using System;

namespace Market.BLL.DTO
{
    public class PagingInfoDTO
    {
        public int TotalItems { get; set; }

        public int ItemsPerPage { get; set; } = 1;

        public int ItemsCurrentPage { get; set; }

        public int CurrentPage { get; set; } = 1;

        public int TotalPages =>
            (int)Math.Ceiling((decimal)TotalItems / ItemsPerPage);
    }
}