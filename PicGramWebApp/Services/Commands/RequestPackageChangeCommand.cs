using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PicGramWebApp.Data;
using PicGramWebApp.Models;
using PicGramWebApp.Services.Logging;

namespace PicGramWebApp.Services.Commands
{
    // Command pattern: encapsulates the package change request operation
    // into a single executable object with its own validation, persistence, and logging.
    public class RequestPackageChangeCommand : ICommand<RequestPackageChangeResult>
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppActionLogger _appActionLogger;
        private readonly ApplicationUser _user;
        private readonly int _requestedPackagePlanId;

        public RequestPackageChangeCommand(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            AppActionLogger appActionLogger,
            ApplicationUser user,
            int requestedPackagePlanId)
        {
            _context = context;
            _userManager = userManager;
            _appActionLogger = appActionLogger;
            _user = user;
            _requestedPackagePlanId = requestedPackagePlanId;
        }

        public async Task<RequestPackageChangeResult> ExecuteAsync()
        {
            var fullUser = await _userManager.Users
                .Include(u => u.PackagePlan)
                .FirstOrDefaultAsync(u => u.Id == _user.Id);

            if (fullUser == null || fullUser.PackagePlan == null)
            {
                return RequestPackageChangeResult.Fail("User or package not found.");
            }

            var today = DateTime.Now.Date;

            var alreadyRequestedToday = await _context.PackageChangeRequests.AnyAsync(r =>
                r.UserId == fullUser.Id &&
                r.RequestedAt.Date == today);

            if (alreadyRequestedToday)
            {
                return RequestPackageChangeResult.Fail("You can only request one package change per day.");
            }

            if (_requestedPackagePlanId == fullUser.PackagePlanId)
            {
                return RequestPackageChangeResult.Fail("You already have that package.");
            }

            var request = new PackageChangeRequest
            {
                UserId = fullUser.Id,
                CurrentPackagePlanId = fullUser.PackagePlanId ?? 0,
                RequestedPackagePlanId = _requestedPackagePlanId,
                RequestedAt = DateTime.Now,
                EffectiveFrom = DateTime.Now.Date.AddDays(1),
                IsApplied = false
            };

            _context.PackageChangeRequests.Add(request);
            await _context.SaveChangesAsync();

            await _appActionLogger.LogForUserAsync(
                fullUser.Id,
                "RequestPackageChange",
                $"FromPackageId={fullUser.PackagePlanId}, ToPackageId={_requestedPackagePlanId}, EffectiveFrom={request.EffectiveFrom:yyyy-MM-dd HH:mm:ss}");

            return RequestPackageChangeResult.Ok();
        }
    }
}