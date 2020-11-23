using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Consumer.Api;
using Consumer.Api.Models;
using Consumer.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.FeatureManagement;

namespace Consumer.API.Controllers
{
    [ApiController]
    [Route("v1/github_organization")]
    public class GithubOrganizationController : ControllerBase
    {
        private readonly IMemoryCache cache;
        private readonly IFeatureManager featureManager;
        private readonly IConfiguration configuration;

        public GithubOrganizationController(IMemoryCache cache, IFeatureManager featureManager, IConfiguration configuration)
        {
            this.cache = cache;
            this.featureManager = featureManager;
            this.configuration = configuration;
        }

        [HttpGet]
        [Route("/repositories/{OrganizationName}")]
        public async Task<ActionResult<List<GithubRepository>>> GetOrganizationRepositories([FromServices] IGithubService githubRepository, string OrganizationName = "ibm")
        {
            if (await isMemoryCachingEnabledAsync() == false)
            {
                cache.Remove(OrganizationName);
            }
            return await cache.GetOrCreateAsync(OrganizationName, RepositoryList =>
            {
                RepositoryList.SlidingExpiration = TimeSpan.FromSeconds(configuration.GetValue<double>("FeatureManagement:CacheExpirationDuration"));
                return githubRepository.getOrganizationRepositories(OrganizationName);
            });
        }

        private async Task<bool> isMemoryCachingEnabledAsync()
        {
            return await featureManager.IsEnabledAsync(nameof(Features.MemoryCache));
        }
    }
}