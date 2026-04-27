using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PicGramWebApp.Data;
using PicGramWebApp.Models;
using PicGramWebApp.Services.Logging;

namespace PicGramWebApp.Services.Packages
{
    public class PackageChangeService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppActionLogger _appActionLogger;

        public PackageChangeService(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            AppActionLogger appActionLogger)
        {
            _context = context;
            _userManager = userManager;
            _appActionLogger = appActionLogger;
        }

        public async Task ApplyPendingChangesAsync(ApplicationUser user)
        {
            var now = DateTime.Now;

            var pendingRequests = await _context.PackageChangeRequests
                .Where(r => r.UserId == user.Id && !r.IsApplied && r.EffectiveFrom <= now)
                .ToListAsync();

            if (!pendingRequests.Any())
                return;

            foreach (var request in pendingRequests)
            {
                user.PackagePlanId = request.RequestedPackagePlanId;
                request.IsApplied = true;

                await _appActionLogger.LogForUserAsync(
                    request.UserId,
                    "ApplyPackageChange",
                    $"Applied requested package change to PackagePlanId={request.RequestedPackagePlanId}");
            }

            await _context.SaveChangesAsync();
        }
    }
}