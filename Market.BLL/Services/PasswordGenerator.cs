using System;
using Market.BLL.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Market.BLL.Services
{
    public class PasswordGenerator : IPasswordGenerator
    {
        private readonly Random _rand = new Random(Environment.TickCount);
        private readonly PasswordOptions _passwordOptions;

        private const string Uppercase = "ABCDEFGHJKLMNOPQRSTUVWXYZ";
        private const string Lowercase = "abcdefghijkmnopqrstuvwxyz";
        private const string Digits = "0123456789";
        private const string NonAlphaNumeric = "!@$?_-";

        public PasswordGenerator(PasswordOptions passwordOptions)
        {
            _passwordOptions = passwordOptions ?? new PasswordOptions
            {
                RequiredLength = 8,
                RequireDigit = true,
                RequireLowercase = true,
                RequireNonAlphanumeric = true,
                RequireUppercase = true
            };
        }

        /// <summary>
        /// Генерирует случайный пароль.
        /// </summary>
        public string Next()
        {
            Span<char> chars = stackalloc char[_passwordOptions.RequiredLength];
            int defaultCount = 0;

            if (_passwordOptions.RequireUppercase)
            {
                defaultCount += RandomPut(chars, Uppercase[_rand.Next(0, Uppercase.Length)]);
            }

            if (_passwordOptions.RequireLowercase)
            {
                defaultCount += RandomPut(chars, Lowercase[_rand.Next(0, Lowercase.Length)]);
            }

            if (_passwordOptions.RequireDigit)
            {
                defaultCount += RandomPut(chars, Digits[_rand.Next(0, Digits.Length)]);
            }

            if (_passwordOptions.RequireNonAlphanumeric)
            {
                defaultCount += RandomPut(chars, NonAlphaNumeric[_rand.Next(0, NonAlphaNumeric.Length)]);
            }

            while (defaultCount < chars.Length)
            {
                switch (_rand.Next(0, 4))
                {
                    case 0:
                        defaultCount += RandomPut(chars, Uppercase[_rand.Next(0, Uppercase.Length)]);
                        break;
                    case 1:
                        defaultCount += RandomPut(chars, Lowercase[_rand.Next(0, Lowercase.Length)]);
                        break;
                    case 2:
                        defaultCount += RandomPut(chars, Digits[_rand.Next(0, Digits.Length)]);
                        break;
                    case 3:
                        defaultCount += RandomPut(chars, NonAlphaNumeric[_rand.Next(0, NonAlphaNumeric.Length)]);
                        break;
                }
            }

            return new string(chars);
        }

        private int RandomPut(Span<char> chars, char value)
        {
            if (chars.Contains(value))
            {
                return 0;
            }

            int randomIndex = _rand.Next(0, chars.Length);

            while (chars[randomIndex] != default)
            {
                randomIndex = _rand.Next(0, chars.Length);
            }

            chars[randomIndex] = value;

            return 1;
        }
    }
}