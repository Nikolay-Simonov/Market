using Market.DAL.Interfaces;
using Microsoft.AspNetCore.Hosting;

namespace Market.Services
{
    internal class ContentEnvironment : IContentEnvironment
    {
        public ContentEnvironment(IWebHostEnvironment webHostEnvironmentEnvironment)
        {
            Path = webHostEnvironmentEnvironment.WebRootPath;
        }

        public string Path { get; }
    }
}