using System.Collections.Generic;

namespace Market.BLL.DTO
{
    public class StaffListDTO
    {
        public IEnumerable<ApplicationUserDTO> Staff { get; set; }

        public PagingInfoDTO PagingInfo { get; set; }
    }
}