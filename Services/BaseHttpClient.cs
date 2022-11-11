using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Akaunting
{
    public class BaseHttpClient
    {
        protected HttpClient _client;

        public BaseHttpClient(HttpClient client)
        {

            _client = client;
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
            ProductInfoHeaderValue productValue = new ProductInfoHeaderValue("AkauntingBot", "1.0");
            _client.DefaultRequestHeaders.UserAgent.Add(productValue);
        }

        protected async Task<T> SendRequest<T>(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
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
            catch (OperationCanceledException ocException)
            {
                Console.WriteLine(DateTime.Now.ToString() + " - " + $"An request operation was cancelled with message {ocException.Message}. " + request.RequestUri);
                T value = default;
                return value;
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString() + " - " + $"Something went wrong: " + ex.Message + " - " + request.RequestUri);
                throw ex;
            }
        }
    }
}
