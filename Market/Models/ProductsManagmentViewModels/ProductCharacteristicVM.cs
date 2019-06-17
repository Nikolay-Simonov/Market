using System.Collections.Generic;

namespace Market.Models.ProductsManagmentViewModels
{
    public class ProductCharacteristicVM
    {
        public string DictionaryName { get; set; }

        public Dictionary<string, string> Characteristics { get; set; }
    }
}