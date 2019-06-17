using System;
using System.Collections.Generic;
using System.Linq;
using Market.BLL.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Market.BLL.Services
{
    public class PasswordGenerator : IPasswordGenerator
    {
        private readonly PasswordOptions _passwordOptions;

        public PasswordGenerator(PasswordOptions passwordOptions)
        {
            _passwordOptions = passwordOptions;

            if (passwordOptions == null)
            {
                _passwordOptions = new PasswordOptions
                {
                    RequiredLength = 8,
                    RequiredUniqueChars = 4,
                    RequireDigit = true,
                    RequireLowercase = true,
                    RequireNonAlphanumeric = true,
                    RequireUppercase = true
                };
            }
        }

        /// <summary>
        /// Генерирует случайный пароль.
        /// </summary>
        public string Next()
        {
            var randomChars = new[] {
                "ABCDEFGHJKLMNOPQRSTUVWXYZ",
                "abcdefghijkmnopqrstuvwxyz",
                "0123456789",
                "!@$?_-"
            };

            var rand = new Random(Environment.TickCount);
            var chars = new List<char>();

            if (_passwordOptions.RequireUppercase)
            {
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[0][rand.Next(0, randomChars[0].Length)]);
            }

            if (_passwordOptions.RequireLowercase)
            {
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[1][rand.Next(0, randomChars[1].Length)]);
            }

            if (_passwordOptions.RequireDigit)
            {
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[2][rand.Next(0, randomChars[2].Length)]);
            }

            if (_passwordOptions.RequireNonAlphanumeric)
            {
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[3][rand.Next(0, randomChars[3].Length)]);
            }

            for (int i = chars.Count; i < _passwordOptions.RequiredLength
                || chars.Distinct().Count() < _passwordOptions.RequiredUniqueChars; i++)
            {
                string rcs = randomChars[rand.Next(0, randomChars.Length)];
                chars.Insert(rand.Next(0, chars.Count),
                    rcs[rand.Next(0, rcs.Length)]);
            }

            return new string(chars.ToArray());
        }
    }
}