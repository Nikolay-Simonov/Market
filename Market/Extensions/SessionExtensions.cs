using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Market.Extensions
{
    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            // byte[] bytes = value == null
            //     ? null
            //     : JsonSerializer.ToUtf8Bytes(value);
            //
            // session.Set(key, bytes);

            if (value == null)
            {
                session.Remove(key);
            }

            string stringValue = JsonConvert.SerializeObject(value);

            session.SetString(key, stringValue);
        }

        public static T Get<T>(this ISession session, string key)
        {
            // byte[] bytes = session.Get(key);
            //
            // return bytes == null
            //     ? default
            //     : JsonSerializer.Parse<T>(bytes);

            string value = session.GetString(key);

            return value == null ? default : JsonConvert.DeserializeObject<T>(value);
        }
    }
}