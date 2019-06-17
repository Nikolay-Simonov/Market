using System.Runtime.Serialization;

namespace Market.BLL.DTO
{
    [DataContract(IsReference = true)]
    public class ProductDTO
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int? CategoryId { get; set; }
        [DataMember]
        public CategoryDTO Category { get; set; }

        [DataMember]
        public int? BrandId { get; set; }

        [DataMember]
        public BrandDTO Brand { get; set; }

        [DataMember]
        public int? CountryId { get; set; }

        [DataMember]
        public CountryDTO Country { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public double Weight { get; set; }

        [DataMember]
        public decimal Price { get; set; }

        [DataMember]
        public string Image { get; set; }

        [DataMember]
        public string Character { get; set; }

        [DataMember]
        public bool Removed { get; set; }
    }
}