namespace Market.Models.ProductsManagmentViewModels
{
    public class ProductDataVM
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Category { get; set; }

        public string Brand { get; set; }

        public string Country { get; set; }

        public double Weight { get; set; }

        public decimal Price { get; set; }

        public bool Removed { get; set; }
    }
}