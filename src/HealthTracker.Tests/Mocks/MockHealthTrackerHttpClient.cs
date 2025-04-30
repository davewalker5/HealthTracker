using System.Net;
using System.Net.Http.Headers;
using HealthTracker.Client.Interfaces;

namespace HealthTracker.Tests.Mocks
{
    internal class MockHealthTrackerHttpClient : IHealthTrackerHttpClient
    {
        private readonly Queue<string> _responses = new();
        private readonly Queue<HttpStatusCode> _statuses = new();

        public Uri BaseAddress { get; set; }
        public HttpRequestHeaders DefaultRequestHeaders { get; private set; } = new HttpRequestMessage().Headers;
        public IList<MockHttpRequest> Requests { get; private set; } = [];

        /// <summary>
        /// Queue a response with a configurable response code
        /// </summary>
        /// <param name="response"></param>
        public void AddResponse(string response, HttpStatusCode status = HttpStatusCode.OK)
        {
            _responses.Enqueue(response);
            _statuses.Enqueue(status);
        }

        /// <summary>
        /// Queue a set of responses with an OK response code
        /// </summary>
        /// <param name="responses"></param>
        /// <param name="status"></param>
        public void AddResponses(IEnumerable<string> responses)
        {
            foreach (string response in responses)
            {
                _responses.Enqueue(response);
                _statuses.Enqueue(HttpStatusCode.OK);
            }
        }

        public Task<HttpResponseMessage> GetAsync(string uri)
            => GetNextResponse(HttpMethod.Get, uri, null);

        public Task<HttpResponseMessage> PostAsync(string uri, HttpContent content)
            => GetNextResponse(HttpMethod.Post, uri, content);

        public Task<HttpResponseMessage> PutAsync(string uri, HttpContent content)
            => GetNextResponse(HttpMethod.Put, uri, content);

        public Task<HttpResponseMessage> DeleteAsync(string uri)
            => GetNextResponse(HttpMethod.Delete, uri, null);

        /// <summary>
        /// Construct and return the next response
        /// </summary>
        /// <param name="method"></param>
        /// <param name="uri"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public Task<HttpResponseMessage> GetNextResponse(HttpMethod method, string uri, HttpContent content)
        {
            // Add the URI to the list of called URIs
            Requests.Add(new MockHttpRequest()
            {
                Method = method,
                Uri = uri,
                Content = content
            });

            // De-queue the next message and its response code
            string responseContent = null;
            HttpStatusCode responseCode = HttpStatusCode.OK;
            if (method != HttpMethod.Delete)
            {
                responseContent = _responses.Dequeue() ?? throw new Exception();
                responseCode = _statuses.Dequeue();
            }

            // Construct an HTTP response
            var response = new HttpResponseMessage
            {
                Content = new StringContent(responseContent ?? ""),
                StatusCode = responseCode
            };

            return Task.FromResult(response);
        }
    }
}