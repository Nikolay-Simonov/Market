using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Market.DAL.EF;
using Market.DAL.Entities;
using Market.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Market.DAL.Repositories
{
    public class CatalogRepository : ICatalogRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CatalogRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Возвращает XML данные характеристик продуктов указанной категории или если та не указана
        /// то характеристики всех продуктов.
        /// </summary>
        /// <param name="categoryName">Имя категории.</param>
        public async Task<string> GetXmlCharacteristics(string categoryName = null)
        {
            await using DbCommand select = _dbContext.Database.GetDbConnection().CreateCommand();

            select.CommandText =
                @"SELECT
                    '<characteristics>' +
                        STRING_AGG(CAST([Products].[Character].query('characteristics/characteristic') AS nvarchar(max)), '') +
                    '</characteristics>' AS Characteristics
                FROM [Products]";

            await select.Connection.OpenAsync();

            if (!string.IsNullOrWhiteSpace(categoryName))
            {
                select.CommandText += " LEFT JOIN [Categories] ON [Products].[CategoryId] = [Categories].[Id] "
                                      + "WHERE [Categories].[Name] = @CategoryName";
                var category = new SqlParameter("CategoryName", SqlDbType.NVarChar)
                {
                    Value = categoryName,
                    Size = Category.NameLength
                };
                select.Parameters.Add(category);
                await select.PrepareAsync();
            }

            return await select.ExecuteScalarAsync() as string;
        }

        /// <summary>
        /// Асинхронно возвращает выборку продуктов, используя SQL.
        /// </summary>
        /// <param name="facetsBuilder">Строитель запроса фасетного поиска.</param>
        /// <returns>Перечисление продуктов.</returns>
        /// <exception cref="ArgumentNullException">SQL код сгеренированный
        /// аргументом "<paramref name="facetsBuilder"/>" является null, empty/whitespace.</exception>
        /// <exception cref="ArgumentException">Свойство Parameters аргумента "<paramref name="facetsBuilder"/>":
        /// не является null и не содержит элементов;
        /// имя одного из SQL параметров является null/empty/whitespace, содержит только префикс "@"
        /// или не найдено в тексте запроса.</exception>
        public IQueryable<Product> FacetsSearch(IFacetsBuilder<Product> facetsBuilder)
        {
            string sql = facetsBuilder.ToString();
            IReadOnlyDictionary<string, object> parameters = facetsBuilder.Parameters;

            if (string.IsNullOrWhiteSpace(sql))
            {
                throw new ArgumentNullException(nameof(facetsBuilder));
            }

            if (parameters == null)
            {
                return _dbContext.Products.FromSqlRaw(sql);
            }

            if (!parameters.Any())
            {
                throw new ArgumentException
                (
                    $"The passed argument \"{nameof(facetsBuilder)}.{nameof(facetsBuilder.Parameters)}\" "
                        + "does not contain any sql parameters.",
                    nameof(parameters)
                );
            }

            var sqlParameters = new SqlParameter[parameters.Count];
            int index = 0;

            foreach (var (name, value) in parameters)
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    throw new ArgumentException
                    (
                        "At least one sql parameter name is null, empty or whitespace.",
                        nameof(parameters)
                    );
                }

                if (ContainsOnlyPrefix(name))
                {
                    throw new ArgumentException
                    (
                        "At least one sql parameter name consists only of the \"@\" prefix.",
                        nameof(parameters)
                    );
                }

                string tempName = name.StartsWith('@') ? name : '@' + name;

                if (!sql.Contains(tempName))
                {
                    throw new ArgumentException
                    (
                        $"The sql parameter with the name \"{tempName}\" was "
                            + "not found in the text of the sql command.",
                        nameof(sql)
                    );
                }

                sqlParameters[index] = new SqlParameter(tempName, value);
                index++;
            }

            return _dbContext.Products.FromSqlRaw(sql, sqlParameters);

            #region Local Functions

            bool ContainsOnlyPrefix(string name) => name == "@"
                || name.StartsWith("@") && string.IsNullOrWhiteSpace(name.Substring(1));

            #endregion
        }

        public IQueryable<Product> Products => _dbContext.Products;
    }
}