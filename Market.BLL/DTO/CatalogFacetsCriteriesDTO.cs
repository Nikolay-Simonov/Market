using System.Collections.Generic;

namespace Market.BLL.DTO
{
    public class CatalogFacetsCriteriesDTO
    {
        public HashSet<string> Countries { get; set; }

        public HashSet<string> Brands { get; set; }

        public Dictionary<string, HashSet<string>> Characteristics { get; set; }
    }
}