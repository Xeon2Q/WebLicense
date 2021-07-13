using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WebLicense.Client.Auxiliary.Extensions
{
    public static class HttpClientExtensions
    {
        #region Private methods

        private static string ToJson<T>(T entity)
        {
            return entity == null ? null : JsonSerializer.Serialize(entity, new JsonSerializerOptions {WriteIndented = false});
        }

        private static T FromJson<T>(string json)
        {
            return string.IsNullOrWhiteSpace(json) ? default : JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions {AllowTrailingCommas = true, PropertyNameCaseInsensitive = true});
        }

        private static string ToQueryString(params (string paramName, object paramValue)[] parameters)
        {
            if (parameters == null || parameters.Length == 0) return null;

            var parts = parameters.Where(q => !string.IsNullOrWhiteSpace(q.paramName)).Select(q => $"{q.paramName.Trim()}={q.paramValue}").ToArray();
            
            return string.Join('&', parts);
        }

        #endregion

        #region Extensions- GET

        public static async Task Get(this HttpClient client, string absoluteUrl, params (string paramName, object paramValue)[] parameters)
        {
            var query = ToQueryString(parameters);
            var response = await client.GetAsync($"{absoluteUrl}{(string.IsNullOrWhiteSpace(query) ? "" : $"?{query}")}");
            response.EnsureSuccessStatusCode();
        }

        public static async Task<T> GetJson<T>(this HttpClient client, string absoluteUrl, params (string paramName, object paramValue)[] parameters)
        {
            var query = ToQueryString(parameters);
            var response = await client.GetAsync($"{absoluteUrl}{(string.IsNullOrWhiteSpace(query) ? "" : $"?{query}")}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return FromJson<T>(json);
        }

        #endregion

        #region Extensions- POST

        public static async Task Post(this HttpClient client, string absoluteUrl, object content, params (string paramName, object paramValue)[] parameters)
        {
            var query = ToQueryString(parameters);
            var message = new HttpRequestMessage(HttpMethod.Post, $"{absoluteUrl}{(string.IsNullOrWhiteSpace(query) ? "" : $"?{query}")}");
            if (content != null) message.Content = new StringContent(ToJson(content), Encoding.UTF8, "application/json");

            var response = await client.SendAsync(message);
            response.EnsureSuccessStatusCode();
        }

        public static async Task<T> PostJson<T>(this HttpClient client, string absoluteUrl, object content, params (string paramName, object paramValue)[] parameters)
        {
            var query = ToQueryString(parameters);
            var message = new HttpRequestMessage(HttpMethod.Post, $"{absoluteUrl}{(string.IsNullOrWhiteSpace(query) ? "" : $"?{query}")}");
            if (content != null) message.Content = new StringContent(ToJson(content), Encoding.UTF8, "application/json");

            var response = await client.SendAsync(message);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return FromJson<T>(json);
        }

        #endregion
    }
}