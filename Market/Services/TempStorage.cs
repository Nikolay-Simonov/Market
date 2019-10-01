using Market.DAL.Interfaces;
using Market.Extensions;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Market.Services
{
    internal class TempStorage<TEntity> : ITempStorage<TEntity>
    {
        private readonly ISession _session;
        private const string Salt = "_25226ee1-c9e7-42ab-a13f-49c8f53d848e";

        private string Key { get; }

        public TempStorage(IHttpContextAccessor httpContextAccessor)
        {
            _session = httpContextAccessor.HttpContext.Session;
            Key = string.Intern(typeof(TEntity).Name + Salt);
        }

        public Task Set(TEntity value) => Task.Run(() => _session.Set(Key, value));

        public Task<TEntity> Get() => Task.Run(() => _session.Get<TEntity>(Key));

        public Task Clear() => Task.Run(() => _session.Remove(Key));
    }
}