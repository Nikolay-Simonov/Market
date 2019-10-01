namespace Market.DAL.Entities
{
    public class CartLine
    {
        public int ProductId { get; set; }

        public Product Product { get; set; }

        public string UserId { get; set; }

        public int Quantity { get; set; }
    }
}