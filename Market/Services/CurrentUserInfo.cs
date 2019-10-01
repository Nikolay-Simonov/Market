using Market.BLL.Interfaces;
using Market.DAL.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Market.BLL.Services
{
    internal class CurrentUserInfo : ICurrentUserInfo
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _accessor;

        public CurrentUserInfo(IHttpContextAccessor accessor,
            UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _accessor = accessor;
        }

        public async Task<string> GetId()
        {
            var user = await _userManager.GetUserAsync(_accessor.HttpContext.User);

            return user?.Id;
        }

        public bool IsAuthenticated => _accessor.HttpContext.User.Identity.IsAuthenticated;

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
                _userManager.Dispose();
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