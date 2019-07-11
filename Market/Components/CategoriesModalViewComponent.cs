using System;
using System.Threading.Tasks;
using Market.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Market.Components
{
    public class CategoriesModalViewComponent : ViewComponent, IDisposable
    {
        private readonly ICatalogManager _catalogManager;

        public CategoriesModalViewComponent(ICatalogManager catalogManager)
        {
            _catalogManager = catalogManager;
        }

        public async Task<IViewComponentResult> InvokeAsync() => View(await _catalogManager.Categories());

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
                _catalogManager.Dispose();
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