﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Market.DAL.Entities
{
    public class Country
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, StringLength(120)]
        public string Name
        {
            get => _name;
            set => _name = value?.Trim();
        }

        private string _name;
    }
}