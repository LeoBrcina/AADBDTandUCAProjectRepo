using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using PicGramWebApp.Models;
using PicGramWebApp.Services.Observers;

namespace PicGramWebApp.Services.Logging
{
    // Logging helper: provides a simple application-facing entry point for logging,
    // while delegating the actual event propagation to the Observer subsystem.
    public class AppActionLogger
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly PhotoActionSubject _photoActionSubject;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AppActionLogger(
            UserManager<ApplicationUser> userManager,
            PhotoActionSubject photoActionSubject,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _photoActionSubject = photoActionSubject;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task LogAsync(string actionType, string? details = null)
        {
            var principal = _httpContextAccessor.HttpContext?.User;
            var user = principal == null ? null : await _userManager.GetUserAsync(principal);

            var actionEvent = new PhotoActionEvent
            {
                UserId = user?.Id,
                ActionType = actionType,
                Details = details
            };

            await _photoActionSubject.NotifyAsync(actionEvent);
        }

        public async Task LogForUserAsync(string? userId, string actionType, string? details = null)
        {
            var actionEvent = new PhotoActionEvent
            {
                UserId = userId,
                ActionType = actionType,
                Details = details
            };

            await _photoActionSubject.NotifyAsync(actionEvent);
        }
    }
}