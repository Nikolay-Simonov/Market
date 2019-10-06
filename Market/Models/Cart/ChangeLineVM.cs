using System.ComponentModel.DataAnnotations;

namespace Market.Models.Cart
{
    public class ChangeLineVM
    {
        public int Id { get; set; }

        public int Quantity { get; set; }

        [Required]
        public string Operation { get; set; }
    }
}
