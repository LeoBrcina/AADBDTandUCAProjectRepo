using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace PicGramWebApp.Services.Adapters
{
    // Concrete Adapter: maps GitHub-specific external login data
    // into the application's standardized ExternalUserInfo model.
    public class GitHubExternalUserAdapter : IExternalUserAdapter
    {
        public ExternalUserInfo Adapt(ExternalLoginInfo info)
        {
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);

            return new ExternalUserInfo
            {
                Email = email ?? "",
                Provider = info.LoginProvider,
                ProviderKey = info.ProviderKey
            };
        }
    }
}