using Microsoft.JSInterop;

namespace Common.Web.JSInterop
{
    public class JsInteropHelper
    {
        private const string _jsGetItem = "localStorage.getItem";
        private const string _jsSetItem = "localStorage.setItem";
        private const string _jsRemoveItem = "localStorage.removeItem";

        private readonly IJSRuntime _jsRuntime;
        public JsInteropHelper(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task SetItem(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new Exception(nameof(key));

            if (string.IsNullOrWhiteSpace(value))
                throw new Exception(nameof(value));

            await _jsRuntime.InvokeVoidAsync(_jsSetItem, key, value);
        }

        public async Task SetManyItems(Dictionary<string, string> keyValuePairs)
        {
            if(keyValuePairs is null)
                throw new ArgumentNullException(nameof(keyValuePairs));

            if (keyValuePairs.Keys.Any(k => String.IsNullOrWhiteSpace(k)))
                throw new Exception("Keys contain a null or white space key.");

            if (keyValuePairs.Values.Any(v => String.IsNullOrWhiteSpace(v)))
                throw new Exception("Values contain a null or white space value.");

            foreach (var kvp in keyValuePairs)
            {
                await SetItem(kvp.Key, kvp.Value);
            }
        }

        public async Task<string> GetItem(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new Exception(nameof(key));
            return await _jsRuntime.InvokeAsync<string>(_jsGetItem, key);
        }

        public async Task<Dictionary<string, string>> GetManyItems(string[] keys)
        {
            if (keys is null)
                throw new ArgumentNullException(nameof(keys));

            if (!keys.Any())
                throw new Exception($"{nameof(keys)} is empty.");

            if (keys.Any(k => String.IsNullOrWhiteSpace(k)))
                throw new Exception("Keys contain a null or white space key.");

            if(await ExistMany(keys))
                throw new Exception("One or more values doesn't exist.");

            var dictionary = new Dictionary<string, string>();
            foreach (var key in keys)
            {
                var value = await GetItem(key);
                dictionary.Add(key, value);
            }
            return dictionary;
        }

        public async Task RemoveItem(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new Exception(nameof(key)); 
            
            await _jsRuntime.InvokeVoidAsync(_jsRemoveItem, key); 
        }

        public async Task RemoveManyItems(string[] keys)
        {
            if(!keys.Any())
                throw new Exception($"{nameof(keys)} is empty.");

            foreach (var key in keys)
            {
                await _jsRuntime.InvokeVoidAsync(_jsRemoveItem, key);
            }
        }

        public async Task<bool> Exists(string key)
            => !String.IsNullOrWhiteSpace(await GetItem(key));

        public async Task<bool> ExistMany(string[] keys)
        {
            if (!keys.Any())
                throw new Exception($"{nameof(keys)} is empty.");

            var emptyValue = false;
            for (int i = 0; i < keys.Length && emptyValue; i++)
            {
                if (await Exists(keys[i]))
                    emptyValue = true;
            }
            return emptyValue;
        }
    }
}
