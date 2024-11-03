using Common.Extensions;
using System.Net.Http.Headers;

namespace Common.Helpers
{
    public static class HttpCallHelper
    {
        private static string[] valuesToEmpty = new string[] { "/", ":" };
        private static string? _baseAddress;
        private static string? _baseController;
        private static string? _controller;
        private static string? _path;
        private static string? _controllerAddress;
        private static string? _bearer;

        public static void Build(string ipAddress, string port, string baseController, string controller, Dictionary<string, string> uriParameters = null, string bearer = null)
        {
            _baseAddress = $"{ipAddress.ReplaceByEmpty(valuesToEmpty)}:{port.ReplaceByEmpty(valuesToEmpty)}";
            _baseController = baseController.ReplaceByEmpty(valuesToEmpty);
            _controller = controller.ReplaceByEmpty(valuesToEmpty);

            _controllerAddress = $"{baseController}/{controller}";
            _path = $"https://{_baseAddress}/api/{_controllerAddress}/";

            //Only in the case of a Get http request
            if (uriParameters is not null)
                AddUriParameters(uriParameters);

            _bearer = bearer;
        }

        /// <summary>
        /// Adds the URI parameters.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <remarks>Only use in Get http request</remarks>
        /// <exception cref="System.ArgumentNullException">The path can't be null or empty. Set it before adding uri parameters.</exception>
        public static void AddUriParameters(Dictionary<string, string> parameters)
        {
            if (String.IsNullOrEmpty(_path))
                throw new ArgumentNullException("The path can't be null or empty. Set it before adding uri parameters.");
            
            //Remove symbol "/" at last index of the _path.
            _path = _path.Remove(_path.Length - 1, 1);
            
            //Add parameters to the uri.
            _path += FormatUriParameters(parameters);
        }

        public static async Task<HttpResponseMessage> GetAsync()
        {
            if (String.IsNullOrEmpty(_path))
                throw new ArgumentNullException("The path can't be null or empty.");

            using var client = new HttpClient();

            client.BaseAddress = _path.ToUri();

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if(_bearer.IsNotNullOrEmpty())
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _bearer);

            Clear();
            return await client.GetAsync(client.BaseAddress);
        }

        public static async Task<HttpResponseMessage> DeleteAsync()
        {
            if (String.IsNullOrEmpty(_path))
                throw new ArgumentNullException("The path can't be null or empty.");

            using var client = new HttpClient();

            client.BaseAddress = _path.ToUri();

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (_bearer.IsNotNullOrEmpty())
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _bearer);

            Clear();
            return await client.DeleteAsync(client.BaseAddress);
        }

        public static async Task<HttpResponseMessage> PutAsync<T>(this T dto)
        {
            if (String.IsNullOrEmpty(_path))
                throw new ArgumentNullException("The path can't be null or empty.");

            using var client = new HttpClient();

            client.BaseAddress = _path.ToUri();

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if(_bearer.IsNotNullOrEmpty())
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _bearer);

            Clear();
            return await client.PutAsync(client.BaseAddress, dto.ToHttpContent(), new CancellationToken());
        }

        public static async Task<HttpResponseMessage> PostAsync<T>(this T dto)
        {
            if (String.IsNullOrEmpty(_path))
                throw new ArgumentNullException("The path can't be null or empty.");
            
            using var client = new HttpClient();

            client.BaseAddress = _path.ToUri();

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if(_bearer.IsNotNullOrEmpty())
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _bearer);

            Clear();
            return await client.PostAsync(client.BaseAddress, dto.ToHttpContent(), new CancellationToken());
        }

        public static async Task<string> ReadAsStringAsync(this HttpResponseMessage response)
            => await response.Content.ReadAsStringAsync();

        public static string DisplayPath()
        {
            Console.WriteLine(_path);
            return _path;
        }

        #region private methods

        private static void Clear()
        {
            _baseAddress = null;
            _baseController = null;
            _controller = null;
            _path = null;
            _controllerAddress = null;
        }

        /// <summary>
        /// Formats the ids to path when there are parameters to pass in the url.
        /// </summary>
        /// <param name="parametersInUrl">The parameters passed in the path.</param>
        /// <remarks>
        /// Examples :
        ///.../GetAll?userId=a77b3e15-8d6f-43ab-b6bc-f1cbaef47b89
        ///.../Get?scriptId=50e1af92-e4ca-4683-b450-8e4fffbf76f7&ownerId=50e1af92-e4ca-4683-b450-8e4fffbf76f7
        ///</remarks>
        private static string FormatUriParameters(this Dictionary<string, string> parametersInUrl)
        {
            var retVal = "?";
            foreach (var parameter in parametersInUrl)
            {
                retVal += $"{parameter.Key}={parameter.Value}&";
            }
            retVal = retVal.Remove(retVal.Length - 1);
            return retVal;
        }

        #endregion private methods
    }
}
