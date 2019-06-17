using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Market.DAL.Entities
{
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// Имя
        /// </summary>
        [Required]
        [StringLength(25)]
        public string FirstName { get; set; }

        /// <summary>
        /// Отчество
        /// </summary>
        [StringLength(25)]
        public string MiddleName { get; set; }

        /// <summary>
        /// Фамилия
        /// </summary>
        [Required]
        [StringLength(25)]
        public string LastName { get; set; }

        /// <summary>
        /// Адрес доставки
        /// </summary>
        [Required]
        [StringLength(150)]
        public string Address { get; set; }

        /// <summary>
        /// Полное имя
        /// </summary>
        [NotMapped]
        public string FullName => string.IsNullOrWhiteSpace(MiddleName)
            ? FirstName + " " + LastName
            : FirstName + " " + MiddleName + " " + LastName;

        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
    }
}