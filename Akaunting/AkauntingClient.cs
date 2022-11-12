using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AkauntingService
{
    public class AkauntingClient
    {
        private string _companyId;
        protected HttpClient _client;

        public AkauntingClient(HttpClient client)
        {
            _client = client;
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
            ProductInfoHeaderValue productValue = new ProductInfoHeaderValue("AkauntingBot", "1.0");
            _client.DefaultRequestHeaders.UserAgent.Add(productValue);
            string url = Environment.GetEnvironmentVariable("akaunting_url");
            _client.BaseAddress = new Uri(url);
            _companyId = Environment.GetEnvironmentVariable("akaunting_company_id");
        }

        public async Task<Status> Ping(CancellationToken cancellationToken)
        {
            string path = "ping";
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, path);

            Status issueBoards = await SendRequest<Status>(request, cancellationToken);

            return issueBoards;
        }

        public async Task<Status> SearchCustomersByEmail(string email, CancellationToken cancellationToken)
        {

            string path = $"contacts?company_id={_companyId}&search=type:customer email:'{email}'";
            path = PrepareUrl(path);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, path);

            Status issueBoards = await SendRequest<Status>(request, cancellationToken);

            return issueBoards;
        }

        private string PrepareUrl(string baseURl)
        {
            int akaunting_page = 0;
            int akaunting_limit = 100;
            return baseURl + $"&page={akaunting_page}&limit={akaunting_limit}";
        }

        private async Task<T> SendRequest<T>(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // "
            using (HttpResponseMessage response = await _client.SendAsync(request,
HttpCompletionOption.ResponseHeadersRead,
cancellationToken))
            {
                if (!response.IsSuccessStatusCode)
                {
                    // inspect the status code
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        // show this to the user
                        // Debug.WriteLine("The requested resource cannot be found." + request.RequestUri);
                        T value = default;
                        return value;
                    }
                    else
                    {
                        response.EnsureSuccessStatusCode();
                    }
                }


                Stream stream = await response.Content.ReadAsStreamAsync();
                JsonSerializerOptions options = new JsonSerializerOptions();
                T result = await JsonSerializer.DeserializeAsync<T>(stream, options);
                return result;

            }
        }

    }
}