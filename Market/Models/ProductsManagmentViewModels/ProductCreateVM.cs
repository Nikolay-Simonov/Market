using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Market.Models.ProductsManagmentViewModels
{
    public class ProductCreateVM
    {
        public SelectList Categories { get; set; }

        public SelectList Brands { get; set; }

        public SelectList Countries { get; set; }

        public SelectList AvailableCharacteristics { get; set; }

        public Dictionary<string, string> Characteristics { get; set; }

        [DisplayName("Image")]
        public IFormFile Image { get; set; }

        [Required]
        public ProductVM Product { get; set; }
    }
}