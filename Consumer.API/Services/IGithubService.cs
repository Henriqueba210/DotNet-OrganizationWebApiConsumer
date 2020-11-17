using System.Collections.Generic;
using System.Threading.Tasks;
using Consumer.Api.Models;

namespace Consumer.Api.Services
{
    public interface IGithubService
    {
        Task<List<GithubRepository>> getOrganizationRepositories(string OrganizationName);
    }
}