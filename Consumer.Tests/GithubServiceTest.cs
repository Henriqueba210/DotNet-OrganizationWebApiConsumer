using Xunit;
using Consumer.Api.Services;
using System.Net.Http;
using Moq;
using System.IO;
using System;
using System.Reflection;
using System.Text;
using Moq.Protected;
using System.Threading.Tasks;
using System.Threading;
using System.Net;

namespace Consumer.Tests
{
    public class GithubServiceTest
    {
        private GithubService githubService;

        public GithubServiceTest()
        {
            var mockData = this.GetType().Assembly.GetManifestResourceStream("Consumer.Tests.Data.testData.json");
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StreamContent(mockData)
            });

            this.githubService = new GithubService(new HttpClient(mockMessageHandler.Object));
        }

        [Fact]
        public async void isOrganizationResponseValid()
        {
            var result = await githubService.getOrganizationRepositories("ibm");
            Assert.NotEmpty(result);
        }
    }
}
