using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Common.Web.SessionManager
{
    public static class SessionExtension 
    {
        //Une session par utilisateur -> loue un block de ram et ce sont les key qui nomme ces block ram.
        //Mettre le T a Exception et le key a null.
        public static T Get<T>(this ISession session, string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new Exception(nameof(key));
            }
            string value = session.GetString(key);
            return string.IsNullOrWhiteSpace(value) ? default : JsonSerializer.Deserialize<T>(session.GetString(key));
        }

        public static void Set<T>(this ISession session, string key, T value)
        {
            if(key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            string json = JsonSerializer.Serialize(value);
            session.SetString(key, json);
        }

        public static bool Exists(this ISession session, string key)
        {
            if(string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (session.Keys.Contains(key))
            {
                if (string.IsNullOrWhiteSpace(session.GetString(key)))
                    return false;
                else
                    return true;
            }
            else 
                return false;
        }

        public static bool Equals<T>(this ISession session, string key, T value)
        {
            if(string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            string valueIn = session.GetString(key);
            return JsonSerializer.Deserialize<T>(valueIn).Equals(value);
        }

        public static void Clear(this ISession session, IOrderedEnumerable<string> keys)
        {
            if (keys is not null || keys.Count() < 1 || keys.Any(k => string.IsNullOrWhiteSpace(k)))
                throw new ArgumentException(nameof(keys));
            
            foreach(string key in keys)
                if (session.Keys.Contains(key))
                    session.Remove(key);
        }
    }
}
