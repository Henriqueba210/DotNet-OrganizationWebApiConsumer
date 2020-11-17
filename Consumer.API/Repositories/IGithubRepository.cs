using System.Collections.Generic;
using System.Threading.Tasks;
using Consumer.Api.Models;

namespace Consumer.Api.Repositories
{
    public interface IGithubRepository
    {
        Task<List<GithubRepository>> getOrganizationRepositories(string OrganizationName);
    }
}