using Microsoft.AspNetCore.Http;
using System;
using System.Text.Json;

namespace Market.Extensions
{
    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            if (value == null)
            {
                session.Remove(key);
            }

            byte[] bytes = JsonSerializer.SerializeToUtf8Bytes<T>(value);

            session.Set(key, bytes);
        }

        public static T Get<T>(this ISession session, string key)
        {
            ReadOnlySpan<byte> bytes = session.Get(key);

            return bytes == null
                ? default
                : JsonSerializer.Deserialize<T>(bytes);
        }
    }
}