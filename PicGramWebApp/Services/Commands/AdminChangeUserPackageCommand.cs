using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PicGramWebApp.Data;
using PicGramWebApp.Models;
using PicGramWebApp.Services.Logging;

namespace PicGramWebApp.Services.Commands
{
    // Command pattern: encapsulates the admin package change action
    // so that the controller delegates the business operation instead of performing it directly.
    public class AdminChangeUserPackageCommand : ICommand<AdminChangeUserPackageResult>
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppActionLogger _appActionLogger;
        private readonly string _targetUserId;
        private readonly int _packagePlanId;

        public AdminChangeUserPackageCommand(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            AppActionLogger appActionLogger,
            string targetUserId,
            int packagePlanId)
        {
            _context = context;
            _userManager = userManager;
            _appActionLogger = appActionLogger;
            _targetUserId = targetUserId;
            _packagePlanId = packagePlanId;
        }

        public async Task<AdminChangeUserPackageResult> ExecuteAsync()
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == _targetUserId);

            if (user == null)
            {
                return AdminChangeUserPackageResult.Fail("User not found.");
            }

            user.PackagePlanId = _packagePlanId;
            await _context.SaveChangesAsync();

            await _appActionLogger.LogAsync(
                "AdminChangedUserPackage",
                $"Changed package of UserId={user.Id} to PackagePlanId={_packagePlanId}");

            return AdminChangeUserPackageResult.Ok();
        }
    }
}