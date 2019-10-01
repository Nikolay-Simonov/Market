using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Market.DAL.EF;
using Market.DAL.Entities;
using Market.DAL.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Market.DAL.Repositories
{
    internal class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _dbContext;

        private const string CartLines = nameof(ApplicationDbContext.CartLines);
        private const string UserId = nameof(CartLine.UserId);
        private const string ProductId = nameof(CartLine.ProductId);

        public CartRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Clear(string userId)
        {
            await _dbContext.Database.ExecuteSqlRawAsync(
                $"DELETE FROM [dbo].[{CartLines}] WHERE [dbo].[{CartLines}].[{UserId}] = '{userId}'");
        }

        public async Task<IEnumerable<ProductLine>> ProductsLines(string userId) => await _dbContext.CartLines
            .Where(cl => cl.UserId == userId).Select(cl => new ProductLine
            {
                ProductId = cl.ProductId,
                Product = cl.Product,
                Quantity = cl.Quantity
            }).ToArrayAsync();

        public async Task<ProductLine> CreateAsync(CartLine line)
        {
            var result = await _dbContext.AddAsync(line);

            return result == null ? null : new ProductLine
            {
                Quantity = result.Entity.Quantity,
                Product = result.Entity.Product,
                ProductId = result.Entity.ProductId
            };
        }

        public ProductLine Update(CartLine line)
        {
            var result = _dbContext.Update(line);

            return result == null ? null : new ProductLine
            {
                Quantity = result.Entity.Quantity,
                Product = result.Entity.Product,
                ProductId = result.Entity.ProductId
            };
        }

        public async Task Remove(int productId, string userId)
        {
            var userIdParam = new SqlParameter("userId", userId);

            var sql = $@"DELETE FROM [dbo].[{CartLines}] WHERE
                      [dbo].[{CartLines}].[{UserId}] = @userId
                      AND [dbo].[{CartLines}].[{ProductId}] = {productId}";

            await _dbContext.Database.ExecuteSqlRawAsync(sql, userIdParam);
        }

        public async Task<ProductLine> ProductLine(int productId, string userId) => await _dbContext.CartLines
            .Where(cl => cl.ProductId == productId || cl.UserId == userId).Select(cl => new ProductLine
            {
                ProductId = cl.ProductId,
                Product = cl.Product,
                Quantity = cl.Quantity
            }).FirstOrDefaultAsync();
    }
}