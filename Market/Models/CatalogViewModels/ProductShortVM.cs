namespace Market.Models.CatalogViewModels
{
    public class ProductShortVM
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Country { get; set; }

        public string Brand { get; set; }

        public string Image { get; set; }

        public decimal Price { get; set; }

        public double Weight { get; set; }
    }
}