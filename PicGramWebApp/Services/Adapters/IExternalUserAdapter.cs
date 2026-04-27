using Microsoft.AspNetCore.Identity;

namespace PicGramWebApp.Services.Adapters
{
    // Adapter pattern: converts external authentication provider data
    // into a unified internal representation used by the application.
    public interface IExternalUserAdapter
    {
        ExternalUserInfo Adapt(ExternalLoginInfo info);
    }
}