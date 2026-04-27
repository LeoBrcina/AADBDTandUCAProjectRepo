using Microsoft.EntityFrameworkCore;
using PicGramWebApp.Data;

namespace PicGramWebApp.Services.Packages.Validation
{
    public class DownloadCountLimitHandler : PackageValidationHandlerBase
    {
        private readonly ApplicationDbContext _context;

        public DownloadCountLimitHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public override async Task<PackageLimitResult> HandleAsync(PackageValidationContext context)
        {
            var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            var downloadCount = await _context.ActionLogs.CountAsync(l =>
                l.UserId == context.User.Id &&
                l.CreatedAt >= startOfMonth &&
                (l.ActionType == "DownloadOriginal" || l.ActionType == "DownloadProcessed"));

            if (downloadCount >= context.PackagePlan.MaxDownloadsPerMonth)
            {
                return PackageLimitResult.Denied("You have reached your package download limit.");
            }

            return await base.HandleAsync(context);
        }
    }
}