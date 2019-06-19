using System;
using System.Threading.Tasks;
using Market.DAL.EF;
using Market.DAL.Entities;
using Market.DAL.Interfaces;

namespace Market.DAL.Repositories
{
    public class EFUnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;

        public EFUnitOfWork(ApplicationDbContext dbContext, IContentEnvironment contentEnvironment)
        {
            _dbContext = dbContext;

            if (dbContext == null)
            {
                throw new ArgumentNullException(nameof(dbContext));
            }

            Products = new EfRepository<Product>(_dbContext);
            Brands = new EfRepository<Brand>(_dbContext);
            Categories = new EfRepository<Category>(_dbContext);
            Countries = new EfRepository<Country>(_dbContext);
            Characteristics = new EfRepository<Characteristic>(_dbContext);
            ProductsImages = new ImageRepository<Product>(contentEnvironment);
        }

        public IRepository<Product> Products { get; }

        public IRepository<Brand> Brands { get; }

        public IRepository<Category> Categories { get; }

        public IRepository<Country> Countries { get; }

        public IRepository<Characteristic> Characteristics { get; }

        public ImageRepositoryBase<Product> ProductsImages { get; }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        #region IDisposable Support
        private bool _disposedValue;

        private void Dispose(bool disposing)
        {
            if (_disposedValue)
            {
                return;
            }

            if (disposing)
            {
                _dbContext.Dispose();
            }

            _disposedValue = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}