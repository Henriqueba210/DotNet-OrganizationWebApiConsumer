using Consumer.Api.Services;
using Consumer.API.Controllers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.FeatureManagement;
using Moq;
using Xunit;

namespace Consumer.Tests
{
    public class GithubOrganizationControllerTest
    {
        private GithubService githubService = GithubServiceTest.mockGithubService();
        private GithubOrganizationController controller = createController();


        public static GithubOrganizationController createController()
        {
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var mockFeatureManager = new Mock<IFeatureManager>();
            var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true)
            .Build();
            return new GithubOrganizationController(memoryCache, mockFeatureManager.Object, configuration);
        }

        [Fact]
        public async void AssertGetCallRunsCompletely()
        {
            var result = await controller.GetOrganizationRepositories(githubService, "ibm");
            Assert.NotEmpty(result.Value);
        }
    }
}