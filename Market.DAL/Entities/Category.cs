using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Market.DAL.Entities
{
    public class Category
    {
        public const int NameLength = 120;

        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, StringLength(NameLength)]
        public string Name
        {
            get => _name;
            set => _name = value?.Trim();
        }

        public virtual ICollection<Product> Products { get; set; }

        private string _name;
    }
}