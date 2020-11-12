using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Consumer.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Consumer.API.Controllers
{
    [ApiController]
    [Route("v1/github")]
    public class GithubApiController : ControllerBase
    {
        private HttpClient client;

        public GithubApiController(HttpClient httpClient)
        {
            client = httpClient;
        }

        [HttpGet]
        [Route("")]
        public async Task<ActionResult<List<Repository>>> Get()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json")
            );
            client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

            var streamTask = client.GetStreamAsync("https://api.github.com/orgs/dotnet/repos");
            var repositories = await JsonSerializer.DeserializeAsync<List<Repository>>(await streamTask);

            return repositories;
        }
    }
}