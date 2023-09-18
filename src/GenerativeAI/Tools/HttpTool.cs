using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Automation.GenerativeAI.Tools
{
    /// <summary>
    /// A tool for HTTP requests
    /// </summary>
    public class HttpTool : FunctionTool, IDisposable
    {
        private readonly HttpClient httpClient;
        private bool disposeclient = false;

        private bool isDisposed = false;
        private HttpTool(HttpClient client, bool dispose) 
        {
            if(client == null)
            {
                httpClient = new HttpClient();
            }
            else
            {
                httpClient = client;
            }
            disposeclient = dispose;
            Name = "ExecuteHttpRequest";
            Description = "Sends HTTP request to the specified URI and returns the response body as a string.";
        }

        /// <summary>
        /// Creates the HttpTool with given client
        /// </summary>
        /// <param name="httpClient">HttpClient to send requests</param>
        /// <returns>HttpTool</returns>
        public static HttpTool WithClient(HttpClient httpClient = default)
        {
            bool dispose = httpClient == null;
            return new HttpTool(httpClient, dispose);
        }

        /// <summary>
        /// Updates the tool with default request headers.
        /// </summary>
        /// <param name="headers">Headers dictionary</param>
        /// <returns>Updated HttpTool</returns>
        public HttpTool WithDefaultRequestHeaders(Dictionary<string, string> headers)
        {
            foreach (var pair in headers)
            {
                httpClient.DefaultRequestHeaders.Add(pair.Key, pair.Value);
            }

            return this;
        }

        /// <summary>
        /// Implements the execution logic
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override async Task<Result> ExecuteCoreAsync(Interfaces.ExecutionContext context)
        {
            var result = new Result() { success = true, output = string.Empty };

            string uri = (string)context["uri"];
            object method = null;
            if (!context.TryGetValue("method", out method) || string.IsNullOrEmpty((string)method)) method = "GET";
            
            object body = null;
            if (!context.TryGetValue("body", out body)) body = string.Empty;

            object token = null;
            var cancellationToken = CancellationToken.None; 
            if(context.TryGetValue("cancellationToken", out token))
            {
                cancellationToken = (CancellationToken)token;
            }

            switch (method)
            {
                case "GET":
                    result.output = await GetAsync(uri, cancellationToken);
                    break;
                case "POST":
                    result.output = await PostAsync(uri, (string)body, cancellationToken);
                    break;
                case "PUT":
                    result.output = await PutAsync(uri, (string)body, cancellationToken);
                    break;
                case "DELETE":
                    result.output = await DeleteAsync(uri, cancellationToken);
                    break;
                default:
                    result.success = false;
                    break;
            }

            return result;
        }

        /// <summary>
        /// Provides the descriptor of the tool
        /// </summary>
        /// <returns></returns>
        protected override FunctionDescriptor GetDescriptor()
        {
            var p1 = new ParameterDescriptor()
            {
                Name = "uri",
                Description = "URI of the request",
                Type = TypeDescriptor.StringType
            };

            var p2 = new ParameterDescriptor()
            {
                Name = "method",
                Description = "HTTP request method such as GET/POST/DELETE, the default is GET",
                Type = new EnumTypeDescriptor(new[] { "GET", "POST", "PUT", "DELETE" }),
                Required = false
            };
            var p3 = new ParameterDescriptor()
            {
                Name = "body",
                Description = "The body of the request",
                Type = TypeDescriptor.StringType,
                Required = false
            };

            var function = new FunctionDescriptor(Name, Description,
                new List<ParameterDescriptor>(new[] { p1, p2, p3 }));
            
            return function;
        }

        /// <summary>
        /// Sends an HTTP GET request to the specified URI and returns the response body as a string.
        /// </summary>
        /// <param name="uri">URI of the request</param>
        /// <param name="cancellationToken">The token to use to request cancellation.</param>
        /// <returns>The response body as a string.</returns>
        public Task<string> GetAsync(string uri, CancellationToken cancellationToken = default) =>
            this.SendRequestAsync(uri, HttpMethod.Get, body: string.Empty, cancellationToken);

        /// <summary>
        /// Sends an HTTP POST request to the specified URI and returns the response body as a string.
        /// </summary>
        /// <param name="uri">URI of the request</param>
        /// <param name="body">The body of the request</param>
        /// <param name="cancellationToken">The token to use to request cancellation.</param>
        /// <returns>The response body as a string.</returns>
        public Task<string> PostAsync(string uri, string body, CancellationToken cancellationToken = default) =>
            this.SendRequestAsync(uri, HttpMethod.Post, body, cancellationToken);

        /// <summary>
        /// Sends an HTTP PUT request to the specified URI and returns the response body as a string.
        /// </summary>
        /// <param name="uri">URI of the request</param>
        /// <param name="body">The body of the request</param>
        /// <param name="cancellationToken">The token to use to request cancellation.</param>
        /// <returns>The response body as a string.</returns>
        public Task<string> PutAsync(string uri, string body, CancellationToken cancellationToken = default) =>
            this.SendRequestAsync(uri, HttpMethod.Put, body, cancellationToken);

        /// <summary>
        /// Sends an HTTP DELETE request to the specified URI and returns the response body as a string.
        /// </summary>
        /// <param name="uri">URI of the request</param>
        /// <param name="cancellationToken">The token to use to request cancellation.</param>
        /// <returns>The response body as a string.</returns>
        public Task<string> DeleteAsync(string uri, CancellationToken cancellationToken = default) =>
            this.SendRequestAsync(uri, HttpMethod.Delete, body: string.Empty, cancellationToken);

        /// <summary>Sends an HTTP request and returns the response content as a string.</summary>
        /// <param name="uri">The URI of the request.</param>
        /// <param name="method">The HTTP method for the request.</param>
        /// <param name="body">Optional request body.</param>
        /// <param name="cancellationToken">The token to use to request cancellation.</param>
        private async Task<string> SendRequestAsync(string uri, HttpMethod method, string body, CancellationToken cancellationToken)
        {
            var contentType = IsJsonString(body) ? "application/json" : "text/plain";
            HttpContent requestContent = string.IsNullOrWhiteSpace(body) ? null : new StringContent(body, Encoding.UTF8, contentType);

            using (var request = new HttpRequestMessage(method, uri) { Content = requestContent })
            {
                using (var response = await this.httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false))
                {
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Disposes/releases the unmanaged resources.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if(isDisposed) return;

            if(disposing && httpClient != null)
            {
                httpClient.Dispose();
            }
            isDisposed = true;
        }

        /// <summary>
        /// Disposes the unmanaged resources and suppresses GC finalization
        /// </summary>
        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }
    }
}
