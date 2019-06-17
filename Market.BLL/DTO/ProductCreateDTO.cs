using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Http;

namespace Market.BLL.DTO
{
    [DataContract(IsReference = true)]
    public class ProductCreateDTO
    {
        [DataMember]
        public Dictionary<string, string> ProductCharacteristics { get; set; }

        [DataMember]
        public IFormFile ProductImage { get; set; }

        [DataMember]
        public ProductDTO Product { get; set; }
    }
}