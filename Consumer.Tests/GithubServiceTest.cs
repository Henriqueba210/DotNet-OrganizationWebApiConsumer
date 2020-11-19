using Xunit;
using Consumer.Api.Services;
using System.Net.Http;
using Moq;
using Moq.Protected;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Linq;
using System.Collections.Generic;

namespace Consumer.Tests
{
    public class GithubServiceTest
    {
        private GithubService githubService = mockGithubService();

        [Fact]
        public async void isOrganizationResponseValid()
        {
            var result = await githubService.getOrganizationRepositories("ibm");
            Assert.NotEmpty(result);
            foreach (var item in result)
            {
                Assert.NotEqual(item.Name, "");
                Assert.NotNull(item.GitHubHomeUrl);
                Assert.NotNull(item.GitUrl);
            }
            Assert.NotNull(result.Any(item => item.Homepage != null));
            Assert.NotNull(result.Any(item => item.Watchers != 0));
            Assert.NotNull(result.Any(item => item.OpenIssues != 0));
            Assert.NotNull(result.Any(item => item.Description != null));
        }

        [Fact]
        public async void testRequestError()
        {
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.ServiceUnavailable,
            });
            var githubService = new GithubService(new HttpClient(mockMessageHandler.Object));

            await Assert.ThrowsAsync<HttpRequestException>(
                async () => await githubService.getOrganizationRepositories("ibm")
            );
        }

        public static GithubService mockGithubService()
        {
            var mockData = typeof(GithubServiceTest).Assembly.GetManifestResourceStream("Consumer.Tests.Data.testData.json");
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StreamContent(mockData)
            });
            return new GithubService(new HttpClient(mockMessageHandler.Object));
        }
    }
}
