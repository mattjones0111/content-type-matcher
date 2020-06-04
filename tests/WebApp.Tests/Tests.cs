namespace WebAppMapping.Tests
{
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.Hosting;
    using Newtonsoft.Json;
    using NUnit.Framework;

    public class Tests
    {
        IHost host;
        HttpClient client;

        [OneTimeSetUp]
        public async Task SetupTests()
        {
            IHostBuilder hostBuilder = new HostBuilder()
                .ConfigureWebHostDefaults(webHost =>
                {
                    webHost.UseTestServer();
                    webHost.UseStartup<Startup>();
                });

            host = await hostBuilder.StartAsync();

            client = host.GetTestClient();
        }

        [Test]
        public async Task CanSayHello()
        {
            HttpResponseMessage response = await client.GetAsync("/hello");

            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();

            Assert.AreEqual("Hello", responseString);
        }

        [Test]
        public async Task CanPostAnythingToAnEndpointWithNoStrictContentType()
        {
            HttpResponseMessage response = await PostJsonAsync(
                "/non-content-type-endpoint",
                default,
                new Widget());

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public async Task CanPostAcceptableContentToEndpoint()
        {
            HttpResponseMessage response = await PostJsonAsync(
                "/endpoint",
                "application/vnd.widget.v1+json",
                new Widget());

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            string responseString = await response.Content.ReadAsStringAsync();
            Assert.AreEqual("Widget 1", responseString);
        }

        [Test]
        public async Task CanPostDifferentAcceptableContentToEndpoint()
        {
            HttpResponseMessage response = await PostJsonAsync(
                "/endpoint",
                "application/vnd.widget.v2+json",
                new Widget());

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            string responseString = await response.Content.ReadAsStringAsync();
            Assert.AreEqual("Widget 2", responseString);
        }

        [Test]
        public async Task UnacceptableContentTypeReturnsErrorCondition()
        {
            HttpResponseMessage response = await PostJsonAsync(
                "/endpoint",
                "application/vnd.widget.v3+json",
                new Widget());

            Assert.AreEqual(HttpStatusCode.UnsupportedMediaType, response.StatusCode);
        }

        [Test]
        public async Task UnsetContentTypeReturnsErrorCondition()
        {
            HttpResponseMessage response = await PostJsonAsync(
                "/endpoint",
                default,
                new Widget());

            Assert.AreEqual(HttpStatusCode.UnsupportedMediaType, response.StatusCode);
        }

        async Task<HttpResponseMessage> PostJsonAsync(
            string url,
            string contentType,
            object state)
        {
            string bodyJson = JsonConvert.SerializeObject(state);

            HttpContent content = new StringContent(bodyJson);

            if (!string.IsNullOrEmpty(contentType))
            {
                content.Headers.ContentType =
                    MediaTypeHeaderValue.Parse(contentType);
            }

            HttpRequestMessage request = new HttpRequestMessage(
                HttpMethod.Post,
                url)
            {
                Content = content
            };

            return await client.SendAsync(request);
        }
    }
}