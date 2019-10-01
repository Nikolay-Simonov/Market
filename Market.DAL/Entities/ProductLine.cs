namespace Market.DAL.Entities
{
    public class ProductLine
    {
        public int ProductId { get; set; }

        public Product Product { get; set; }

        public int Quantity { get; set; }
    }
}