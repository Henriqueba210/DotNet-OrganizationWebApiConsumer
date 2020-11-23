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
        private static Mock<IConfiguration> configuration = new Mock<IConfiguration>();

        public static GithubOrganizationController createController()
        {
            var memoryCache = new MemoryCache(new MemoryCacheOptions());

            setupMemoryCacheFlagReturnValue(false);

            setupMemoryCacheDuration(30);

            return new GithubOrganizationController(memoryCache, featureManager.Object, configuration.Object);
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
            setupMemoryCacheDuration(30);
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

        private static void setupMemoryCacheDuration(double duration)
        {
            var mockConfigurationSection = new Mock<IConfigurationSection>();
            mockConfigurationSection.Setup(x => x.Key).Returns("FeatureManagement:CacheExpirationDuration");
            mockConfigurationSection.Setup(x => x.Value).Returns(duration.ToString());
            configuration.Setup(m => m.GetSection("FeatureManagement:CacheExpirationDuration"))
                .Returns((string key) => mockConfigurationSection.Object);
        }
    }
}