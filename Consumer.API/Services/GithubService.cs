using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Consumer.Api.Models;

namespace Consumer.Api.Services
{
    public class GithubService : IGithubService
    {
        private readonly HttpClient client;

        public GithubService(HttpClient client)
        {
            this.client = client;
        }
        public async Task<List<GithubRepository>> getOrganizationRepositories(string OrganizationName)
        {
            try
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json")
                );
                client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

                string Url = $"https://api.github.com/orgs/{OrganizationName}/repos";
                var streamTask = client.GetStreamAsync(Url);
                return await JsonSerializer.DeserializeAsync<List<GithubRepository>>(await streamTask);
            }
            catch
            {
                return new List<GithubRepository>();
            }
        }
    }
}