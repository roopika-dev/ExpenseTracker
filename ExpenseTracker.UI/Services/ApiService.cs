using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.UI.Services
{
    public class ApiService
    {
        private readonly string baseUrl = "https://localhost:7138/api/";

        private async Task<string> HandleResponse(HttpResponseMessage response)
        {
            // 🔥 HANDLE UNAUTHORIZED (TOKEN EXPIRED)
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return "UNAUTHORIZED";
            }

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetAsync(string url, string token = null)
        {
            using (HttpClient client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);

                var response = await client.GetAsync(baseUrl + url);

                return await HandleResponse(response);
            }
        }

        public async Task<string> PostAsync(string url, object data, string token = null)
        {
            using (HttpClient client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);

                var content = new StringContent(
                    JsonConvert.SerializeObject(data),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await client.PostAsync(baseUrl + url, content);

                return await HandleResponse(response);
            }
        }

        public async Task<string> PutAsync(string url, object data, string token = null)
        {
            using (HttpClient client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);

                var content = new StringContent(
                    JsonConvert.SerializeObject(data),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await client.PutAsync(baseUrl + url, content);

                return await HandleResponse(response);
            }
        }

        public async Task<string> DeleteAsync(string url, string token = null)
        {
            using (HttpClient client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);

                var response = await client.DeleteAsync(baseUrl + url);

                return await HandleResponse(response);
            }
        }
    }
}