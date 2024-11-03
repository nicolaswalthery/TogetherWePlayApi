using Microsoft.JSInterop;
using System.Text.Json;

namespace Common.Web.JSInterop
{
    public class SessionManager<T>
        where T : class
    {
        private readonly string _sessionUserKey = "SessionUser";
        private JsInteropHelper _jsInteropHelper;

        public SessionManager(IJSRuntime jsRuntime)
        {
            _jsInteropHelper = new JsInteropHelper(jsRuntime);
        }

        #region Session Managment Methods

        public async Task ClearSession()
            => await _jsInteropHelper.RemoveItem(_sessionUserKey);
        
        public async Task<T> GetSession()
        {
            string value = await _jsInteropHelper.GetItem(_sessionUserKey);
            return string.IsNullOrWhiteSpace(value) ? default : JsonSerializer.Deserialize<T>(value);
        }

        public async Task SetSession(T value)
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));
            string json = JsonSerializer.Serialize(value);
            await _jsInteropHelper.SetItem(_sessionUserKey, json);
        }

        public async Task ReplaceSessionUserBy(T value)
        {
            await ClearSession();
            await SetSession(value);
        }

        public async Task<bool> IsAuth() => await Exists(_sessionUserKey);

        #endregion Session Managment Methods

        public async Task<T> Get<T>(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new Exception(nameof(key));
            string value = await _jsInteropHelper.GetItem(key);
            return string.IsNullOrWhiteSpace(value) ? default : JsonSerializer.Deserialize<T>(value);
        }

        public async Task Set<T>(string key, T value)
        {
            if(key is null)
                throw new ArgumentNullException(nameof(key));
            string json = JsonSerializer.Serialize(value);
            await _jsInteropHelper.SetItem(key, json);
        }

        public async Task<bool> Exists(string key)
        {
            if(string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            return await _jsInteropHelper.Exists(key);
        }

        public async Task<bool> Equals<T>(string key, T value)
        {
            if(string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            string valueIn = await _jsInteropHelper.GetItem(key);
            return JsonSerializer.Deserialize<T>(valueIn).Equals(value);
        }

        public async Task Clear(IOrderedEnumerable<string> keys)
        {
            if (keys is not null || keys.Count() < 1 || keys.Any(k => string.IsNullOrWhiteSpace(k)))
                throw new ArgumentException(nameof(keys));
            
            foreach(string key in keys)
                if (await _jsInteropHelper.Exists(key))
                    await _jsInteropHelper.RemoveItem(key);
        }
    }
}
