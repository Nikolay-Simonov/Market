using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using NToastNotify.Helpers;

namespace NToastNotify
{
    internal class TempDataWrapper : ITempDataWrapper
    {
        private readonly HttpContext _context;
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly ITempDataDictionaryFactory _tempDataDictionaryFactory;

        public TempDataWrapper(ITempDataDictionaryFactory tempDataDictionaryFactory, IHttpContextAccessor httpContextAccessor)
        {
            _tempDataDictionaryFactory = tempDataDictionaryFactory;
            _context = httpContextAccessor.HttpContext;
            _serializerSettings = GetSerializerSettings();
        }

        private JsonSerializerSettings GetSerializerSettings()
        {
            return new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            };
        }

        private ITempDataDictionary TempData => _tempDataDictionaryFactory?.GetTempData(_context);

        public T Get<T>(string key) where T : class
        {
            return TempData.ContainsKey(key)
                ? JsonConvert.DeserializeObject<T>(TempData[key] as string)
                : default;
        }

        public T Peek<T>(string key) where T : class
        {
            return TempData.ContainsKey(key)
                ? JsonConvert.DeserializeObject<T>(TempData.Peek(key) as string)
                : default;
        }

        public void Add(string key, object value)
        {
            TempData[key] = value.ToJson();
        }

        public bool Remove(string key)
        {
            return TempData.ContainsKey(key) && TempData.Remove(key);
        }
    }
}