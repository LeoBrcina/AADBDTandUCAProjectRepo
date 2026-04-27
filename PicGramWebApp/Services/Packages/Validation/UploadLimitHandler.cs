using Microsoft.EntityFrameworkCore;
using PicGramWebApp.Data;

namespace PicGramWebApp.Services.Packages.Validation
{
    public class UploadCountLimitHandler : PackageValidationHandlerBase
    {
        private readonly ApplicationDbContext _context;

        public UploadCountLimitHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public override async Task<PackageLimitResult> HandleAsync(PackageValidationContext context)
        {
            var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            var uploadedPhotos = await _context.Photos.CountAsync(p =>
                p.UserId == context.User.Id &&
                p.UploadedAt >= startOfMonth);

            if (uploadedPhotos >= context.PackagePlan.MaxUploadsPerMonth)
            {
                return PackageLimitResult.Denied("You have reached your package upload limit.");
            }

            return await base.HandleAsync(context);
        }
    }
}