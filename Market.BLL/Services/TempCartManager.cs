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
    internal class TempCartManager : AppManagerBase, ICartManager
    {
        private readonly ITempStorage<List<ProductLine>> _storage;

        public TempCartManager(ITempStorage<List<ProductLine>> storage, IUnitOfWork database) : base(database)
        {
            _storage = storage;
        }

        public async Task<OperationResult> Add(int id, int quantity = 1)
        {
            if (quantity < 1)
            {
                quantity = 1;
            }

            var lines = await _storage.Get() ?? new List<ProductLine>();

            if (!await Database.Products.AnyAsync(p => p.Id == id && !p.Removed))
            {
                lines.RemoveAll(p => p.ProductId == id);
                await _storage.Set(lines);

                return new OperationResult(ResultType.Warning, "Product not found");
            }

            ProductLine productLine = lines.Find(l => l.ProductId == id);

            if (productLine != null)
            {
                productLine.Quantity += quantity;
                await _storage.Set(lines);

                return new OperationResult(ResultType.Success); 
            }

            var line = new ProductLine
            {
                ProductId = id,
                Quantity = quantity,
                Product = await Database.Products.Include(p => p.Brand).Include(p => p.Country)
                    .FirstOrDefaultAsync(p => p.Id == id)
            };

            if (line.Product.Brand != null)
            {
                line.Product.Brand.Products = null;
            }

            lines.Add(line);
            await _storage.Set(lines);

            return new OperationResult(ResultType.Success);
        }

        public async Task<OperationResult> Clear()
        {
            await _storage.Clear();

            return new OperationResult(ResultType.Success);
        }

        public async Task<ProductLineDTO> ProductLine(int id)
        {
            var lines = await _storage.Get();
            var line = lines?.Find(l => l.ProductId == id);

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
            var lines = await _storage.Get();

            return lines == null ? new List<ProductLineDTO>() : lines.Select(line => new ProductLineDTO
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

        public async Task<OperationResult> Remove(int id, int quantity = 1)
        {
            if (quantity < 1)
            {
                quantity = 1;
            }

            var lines = await _storage.Get();

            if (lines == null)
            {
                return new OperationResult(ResultType.Info, "Cart is empty");
            }

            if (!await Database.Products.AnyAsync(p => p.Id == id))
            {
                lines.RemoveAll(p => p.ProductId == id);
                await _storage.Set(lines);

                return new OperationResult(ResultType.Warning, "Product not found");
            }

            ProductLine productLine = lines.Find(l => l.ProductId == id);

            if (productLine == null)
            {
                return new OperationResult(ResultType.Warning, "Product not found in cart");
            }

            int total = productLine.Quantity - quantity;

            if (total < 1)
            {
                lines.RemoveAll(p => p.ProductId == id);
                await _storage.Set(lines);

                return new OperationResult(ResultType.Success);
            }

            productLine.Quantity = total;
            await _storage.Set(lines);

            return new OperationResult(ResultType.Success);
        }

        public async Task<OperationResult> RemoveLine(int id)
        {
            var lines = await _storage.Get();

            if (lines == null)
            {
                return new OperationResult(ResultType.Info, "Cart is empty");
            }

            lines.RemoveAll(l => l.ProductId == id);
            await _storage.Set(lines);

            return new OperationResult(ResultType.Success);
        }
    }
}
