using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PicGramWebApp.Models;
using PicGramWebApp.Services.Logging;

namespace PicGramWebApp.Areas.Identity.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;
        private readonly AppActionLogger _appActionLogger;

        public LogoutModel(
            SignInManager<ApplicationUser> signInManager,
            ILogger<LogoutModel> logger,
            AppActionLogger appActionLogger)
        {
            _signInManager = signInManager;
            _logger = logger;
            _appActionLogger = appActionLogger;
        }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            await _appActionLogger.LogAsync("Logout", "User logged out");

            await _signInManager.SignOutAsync();

            _logger.LogInformation("User logged out.");

            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }

            return RedirectToPage();
        }
    }
}