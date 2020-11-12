using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
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
        public async Task<string> Get()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json")
            );
            client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

            var stringTask = client.GetStringAsync("https://api.github.com/orgs/dotnet/repos");

            return await stringTask;
        }
    }
}