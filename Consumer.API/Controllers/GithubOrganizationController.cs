using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Consumer.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Consumer.API.Controllers
{
    [ApiController]
    [Route("v1/github_organization")]
    public class GithubOrganizationController : ControllerBase
    {
        private HttpClient client;
        private IMemoryCache cache;

        public GithubOrganizationController(HttpClient httpClient, IMemoryCache cache)
        {
            client = httpClient;
            this.cache = cache;
        }

        [HttpGet]
        [Route("/repositories/")]
        public async Task<ActionResult<List<Repository>>> GetOrganizationRepositories(string OrganizationName = "ibm")
        {
            var cacheEntry = await
                cache.GetOrCreateAsync(OrganizationName, RepositoryList =>
                {
                    RepositoryList.SlidingExpiration = TimeSpan.FromSeconds(3);
                    return getRepositories(OrganizationName);
                });

            return cacheEntry;
        }


        private async Task<List<Repository>> getRepositories(string OrganizationName)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json")
            );
            client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

            string Url = $"https://api.github.com/orgs/{OrganizationName}/repos";
            var streamTask = client.GetStreamAsync(Url);
            Console.WriteLine("Created request to server");
            return await JsonSerializer.DeserializeAsync<List<Repository>>(await streamTask);
        }
    }
}