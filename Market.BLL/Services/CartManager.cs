using Market.BLL.DTO;
using Market.BLL.Interfaces;
using Market.DAL.Entities;
using Market.DAL.Enums;
using Market.DAL.Interfaces;
using Market.DAL.Results;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Market.BLL.Services
{
    internal class CartManager : AppManagerBase, ICartManager
    {
        private readonly ICurrentUserInfo _userInfo;

        public CartManager(IUnitOfWork database, ICurrentUserInfo userInfo) : base(database)
        {
            _userInfo = userInfo;
        }

        public async Task<ProductLineDTO> ProductLine(int id)
        {
            var line = await Database.Cart.ProductLine(id, await _userInfo.GetId());

            return line == null || line.Product == null ? null : new ProductLineDTO
            {
                Id = line.ProductId,
                Brand = line.Product.Brand?.Name,
                Country = line.Product.Country?.Name,
                Name = line.Product.Name,
                Image = line.Product.Image,
                Price = line.Product.Price,
                Quantity = line.Quantity,
                Weight = line.Product.Weight
            };
        }

        public async Task<List<ProductLineDTO>> ProductsLines()
        {
            return (await Database.Cart.ProductsLines(await _userInfo.GetId()))
                .Where(l => l != null && l.Product != null)
                .Select(line => new ProductLineDTO
                {
                    Id = line.ProductId,
                    Brand = line.Product.Brand?.Name,
                    Country = line.Product.Country?.Name,
                    Name = line.Product.Name,
                    Image = line.Product.Image,
                    Price = line.Product.Price,
                    Quantity = line.Quantity,
                    Weight = line.Product.Weight
                }).ToList();
        }

        public async Task<OperationResult> Add(int id, int quantity = 1)
        {
            if (quantity < 1)
            {
                quantity = 1;
            }

            string userId = await _userInfo.GetId();

            if (userId == null)
            {
                await Database.Cart.Clear(userId);
                await Database.SaveChangesAsync();

                return new OperationResult(ResultType.Warning, "User not found");
            }

            if (!await Database.Products.AnyAsync(p => p.Id == id))
            {
                await Database.Cart.Remove(id, userId);
                await Database.SaveChangesAsync();

                return new OperationResult(ResultType.Warning, "Product not found");
            }

            ProductLine productLine = await Database.Cart.ProductLine(id, userId);

            if (productLine == null)
            {
                ProductLine createResult = await Database.Cart.CreateAsync(new CartLine
                {
                    ProductId = id,
                    Quantity = quantity,
                    UserId = userId,
                });
                await Database.SaveChangesAsync();

                if (createResult == null)
                {
                    return new OperationResult(ResultType.Error, "Failed to add product");
                }

                return new OperationResult(ResultType.Success);
            }

            ProductLine updateResult = Database.Cart.Update(new CartLine
            {
                ProductId = id,
                Quantity = productLine.Quantity + quantity,
                UserId = userId
            });
            await Database.SaveChangesAsync();

            if (updateResult != null)
            {
                return new OperationResult(ResultType.Success);
            }

            await Database.Cart.Remove(id, userId);
            await Database.SaveChangesAsync();

            return new OperationResult(ResultType.Error, "Failed to add product");
        }

        public async Task<OperationResult> Remove(int id, int quantity = 1)
        {
            if (quantity < 1)
            {
                quantity = 1;
            }

            string userId = await _userInfo.GetId();

            if (userId == null)
            {
                await Database.Cart.Clear(userId);
                await Database.SaveChangesAsync();

                return new OperationResult(ResultType.Warning, "User not found");
            }

            if (!await Database.Products.AnyAsync(p => p.Id == id))
            {
                await Database.Cart.Remove(id, userId);
                await Database.SaveChangesAsync();

                return new OperationResult(ResultType.Warning, "Product not found");
            }

            ProductLine productLine = await Database.Cart.ProductLine(id, userId);

            if (productLine == null)
            {
                return new OperationResult(ResultType.Warning, "Product not found in cart");
            }

            int total = productLine.Quantity - quantity;

            if (total < 1)
            {
                await Database.Cart.Remove(id, userId);
                await Database.SaveChangesAsync();

                return new OperationResult(ResultType.Success);
            }

            ProductLine updateResult = Database.Cart.Update(new CartLine
            {
                ProductId = id,
                Quantity = total,
                UserId = userId
            });
            await Database.SaveChangesAsync();

            if (updateResult != null)
            {
                return new OperationResult(ResultType.Success);
            }

            await Database.Cart.Remove(id, userId);
            await Database.SaveChangesAsync();

            return new OperationResult(ResultType.Error, "Failed to remove product from cart");
        }

        public async Task<OperationResult> RemoveLine(int id)
        {
            string userId = await _userInfo.GetId();

            if (userId == null)
            {
                return new OperationResult(ResultType.Warning, "User not found");
            }

            await Database.Cart.Remove(id, userId);
            await Database.SaveChangesAsync();

            return new OperationResult(ResultType.Success);
        }

        public async Task<OperationResult> Clear()
        {
            string userId = await _userInfo.GetId();

            if (userId == null)
            {
                return new OperationResult(ResultType.Warning, "User not found");
            }

            await Database.Cart.Clear(userId);
            await Database.SaveChangesAsync();

            return new OperationResult(ResultType.Success);
        }

        #region IDisposable Support

        private bool _disposedValue;

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (_disposedValue)
            {
                return;
            }

            if (disposing)
            {
                _userInfo.Dispose();
            }

            _disposedValue = true;
        }

        #endregion
    }
}