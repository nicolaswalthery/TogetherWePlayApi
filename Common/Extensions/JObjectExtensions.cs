using Newtonsoft.Json.Linq;

namespace Common.Extensions
{
    public static class JObjectExtensions
    {
        public static JObject JsonToJObject(this string json)
            => JObject.Parse(json);

        public static string GetValue(this string json, string key) => json.JsonToJObject()[key].Value<string>();

        public static string GetValue(this string json, string parentKey, string childKey) => json.JsonToJObject()[parentKey][childKey].Value<string>();

        public static string GetValue(this JToken jObject, string key) => jObject[key].Value<string>();

        public static string GetValue(this JToken jObject, string parentKey, string childKey) => jObject[parentKey][childKey].Value<string>();

        public static List<string> GetValues(this JToken jObject, string parentKey, string childKey) 
        {
            if(jObject is null || parentKey.IsNullOrEmptyOrWhiteSpace() || childKey.IsNullOrEmptyOrWhiteSpace())
                return null;

            var retVal = new List<string>();
            var jObjects = jObject[parentKey][childKey].Values().ToList();
            var count = jObjects.Count();
            for (int i = 0; i < count; i++)
            {
                retVal.Add(jObjects.GetThenRemove().Value<string>());
            }

            return retVal;
        } 
    }
}
