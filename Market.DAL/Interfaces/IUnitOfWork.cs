using System;
using System.Threading.Tasks;
using Market.DAL.Entities;

namespace Market.DAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Product> Products { get; }

        IRepository<Brand> Brands { get; }

        IRepository<Category> Categories { get; }

        IRepository<Country> Countries { get; }

        IRepository<Characteristic> Characteristics { get; }

        ImageRepositoryBase<Product> ProductsImages { get; }

        ICatalogRepository Catalog { get; }

        ICartRepository Cart { get; }

        Task SaveChangesAsync();
    }
}