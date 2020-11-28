using Xunit;
using Consumer.Api.Services;
using System.Net.Http;
using Moq;
using Moq.Protected;
using System.Threading.Tasks;
using System.Threading;
using System.Net;

namespace Consumer.Tests
{
    public class GithubServiceTest
    {
        private static Mock<HttpMessageHandler> mockMessageHandler = new Mock<HttpMessageHandler>();
        private GithubService githubService = new GithubService(new HttpClient(mockMessageHandler.Object));

        [Fact]
        public async void isOrganizationResponseValid()
        {
            setupSuccessfulHttpResponse();

            var result = await githubService.getOrganizationRepositories("ibm");
            Assert.NotEmpty(result);
            foreach (var item in result)
            {
                Assert.NotEqual(item.Name, string.Empty);
                Assert.NotNull(item.GitHubHomeUrl);
                Assert.NotNull(item.GitUrl);
            }
            Assert.Contains(result, item => item.Homepage != null);
            Assert.Contains(result, item => item.Watchers != 0);
            Assert.Contains(result, item => item.OpenIssues != 0);
            Assert.Contains(result, item => item.Description != null);

            mockMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                ItExpr.IsAny<CancellationToken>()
            );
        }

        [Fact]
        public async void testRequestError()
        {
            setupServerUnavailableHttpResponse();

            await Assert.ThrowsAsync<HttpRequestException>(
                async () => await githubService.getOrganizationRepositories("ibm")
            );
        }

        private void setupSuccessfulHttpResponse()
        {
            mockMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() =>
            {
                var mockData = typeof(GithubServiceTest).Assembly.GetManifestResourceStream("Consumer.Tests.Data.testData.json");

                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StreamContent(mockData)
                };
            });
        }

        private void setupServerUnavailableHttpResponse()
        {
            mockMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.ServiceUnavailable,
            });
        }
    }
}
