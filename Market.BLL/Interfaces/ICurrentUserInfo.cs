using System;
using System.Threading.Tasks;

namespace Market.BLL.Interfaces
{
    public interface ICurrentUserInfo : IDisposable
    {
        Task<string> GetId();

        bool IsAuthenticated { get; }
    }
}
