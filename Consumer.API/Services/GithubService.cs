using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Consumer.Api.Models;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Consumer.Api.Services
{
    public class GithubService : IGithubService
    {
        private readonly HttpClient client;
        private readonly ILogger<GithubService> logger;

        public GithubService(HttpClient client, ILogger<GithubService> logger)
        {
            this.client = client;
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json")
            );
            client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");
            this.logger = logger;
        }
        public async Task<List<GithubRepository>> getOrganizationRepositories(string OrganizationName)
        {
            string Url = $"https://api.github.com/orgs/{OrganizationName}/repos";
            var githubResponse = await client.GetAsync(Url);
            if (githubResponse.IsSuccessStatusCode)
            {
                try
                {
                    return await JsonSerializer.DeserializeAsync<List<GithubRepository>>(await githubResponse.Content.ReadAsStreamAsync());
                }
                catch (JsonException) { throw createStatusException(HttpStatusCode.InternalServerError, "Error while reading Github response"); }
                catch (System.NotSupportedException) { throw createStatusException(HttpStatusCode.InternalServerError, "Unkown internal error during parsing of response"); }
                catch (System.ArgumentNullException) { throw createStatusException(HttpStatusCode.InternalServerError, "No response from Github while reading response body"); }
            }
            else
            {
                string errorMessage;
                switch (githubResponse.StatusCode)
                {
                    case System.Net.HttpStatusCode.NotFound:
                        errorMessage = "The requested organization was not found";
                        break;
                    case System.Net.HttpStatusCode.ServiceUnavailable:
                        errorMessage = "We were unable to contact Github Servers";
                        break;
                    case System.Net.HttpStatusCode.TooManyRequests:
                        errorMessage = "There were too many requests made recently, please try again later";
                        break;
                    default:
                        errorMessage = "An unknown error ocurred while trying to process the request";
                        break;
                }
                throw createStatusException(githubResponse.StatusCode, errorMessage);
            }
        }

        private HttpStatusException createStatusException(HttpStatusCode statusCode, string errorMessage)
        {
            var exception = new HttpStatusException(statusCode, errorMessage);
            logger.LogError(exception, errorMessage);
            return exception;
        }
    }
}