namespace Market.Models
{
    public class ProductLineVM
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Country { get; set; }

        public string Brand { get; set; }

        public string Image { get; set; }

        public decimal Price { get; set; }

        public double Weight { get; set; }

        public int Quantity { get; set; }

        public decimal? TotalPrice => Price * Quantity;
    }
}
