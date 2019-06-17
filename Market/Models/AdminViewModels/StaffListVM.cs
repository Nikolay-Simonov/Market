using System.Collections.Generic;
using System.ComponentModel;

namespace Market.Models.AdminViewModels
{
    public class StaffListVM
    {
        public PagingInfoVM PagingInfo { get; set; }

        public IEnumerable<StaffDataVM> Staff { get; set; }

        [DisplayName("Name or email")]
        public string NameOrEmail { get; set; }
    }
}