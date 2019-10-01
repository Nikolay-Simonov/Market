using System;
using Market.DAL.Interfaces;

namespace Market.BLL.Services
{
    internal abstract class AppManagerBase : IDisposable
    {
        protected AppManagerBase(IUnitOfWork database)
        {
            Database = database;
        }

        protected IUnitOfWork Database { get; }

        #region IDisposable Support

        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue)
            {
                return;
            }

            if (disposing)
            {
                Database.Dispose();
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