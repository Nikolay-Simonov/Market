using System;

namespace Market.Models
{
    public class PagingInfoVM
    {
        public int TotalItems { get; set; }

        public int ItemsPerPage { get; set; } = 1;

        public int ItemsCurrentPage { get; set; }

        public int CurrentPage { get; set; } = 1;

        public int TotalPages =>
            (int)Math.Ceiling((decimal)TotalItems / ItemsPerPage);
    }
}