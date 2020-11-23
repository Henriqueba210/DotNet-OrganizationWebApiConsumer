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
        private static Mock<IFeatureManager> featureManager = new Mock<IFeatureManager>();


        public static GithubOrganizationController createController()
        {
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true)
                .Build();

            setupMemoryCacheFlagReturnValue(false);

            return new GithubOrganizationController(memoryCache, featureManager.Object, configuration);
        }

        [Fact]
        public async void AssertGetCallRunsCompletely()
        {
            var result = await controller.GetOrganizationRepositories(githubService, "ibm");
            Assert.NotEmpty(result.Value);
        }

        [Fact]
        public async void ControllerShouldReturnCachedResponse()
        {
            setupMemoryCacheFlagReturnValue(true);
            var result = await controller.GetOrganizationRepositories(githubService, "ibm");
            var result2 = await controller.GetOrganizationRepositories(githubService, "ibm");
            Assert.Equal(result.Value, result2.Value);
        }

        [Fact]
        public async void ControllerShouldNotReturnCachedResponse()
        {
            setupMemoryCacheFlagReturnValue(false);
            var result = await controller.GetOrganizationRepositories(githubService, "ibm");
            var result2 = await controller.GetOrganizationRepositories(githubService, "ibm");
            Assert.NotEqual(result.Value, result2.Value);
        }

        private static void setupMemoryCacheFlagReturnValue(bool returnValue)
        {
            featureManager.Setup(m => m.IsEnabledAsync(It.IsAny<string>()))
            .ReturnsAsync((string feature) => returnValue);
        }
    }
}