using Consumer.Api.Services;
using Consumer.API.Controllers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
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
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true)
                .Build();
            var services = new ServiceCollection();
            services
                .AddSingleton(configuration)
                .AddFeatureManagement();

            ServiceProvider serviceProvider = services.BuildServiceProvider();

            IFeatureManager featureManager = serviceProvider.GetRequiredService<IFeatureManager>();

            return new GithubOrganizationController(memoryCache, featureManager, configuration);
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
            var result = await controller.GetOrganizationRepositories(githubService, "ibm");
            var result2 = await controller.GetOrganizationRepositories(githubService, "ibm");
            Assert.Equal(result.Value, result2.Value);
        }
    }
}