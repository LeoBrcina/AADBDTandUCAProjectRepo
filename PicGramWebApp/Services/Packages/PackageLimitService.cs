using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PicGramWebApp.Data;
using PicGramWebApp.Models;
using PicGramWebApp.Services.Packages.Validation;

namespace PicGramWebApp.Services.Packages
{
    public class PackageLimitService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PackageLimitService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<PackageLimitResult> CanUploadAsync(ApplicationUser user, long newFileSize)
        {
            var fullUser = await _userManager.Users
                .Include(u => u.PackagePlan)
                .FirstOrDefaultAsync(u => u.Id == user.Id);

            if (fullUser == null || fullUser.PackagePlan == null)
                return PackageLimitResult.Denied("No package is assigned to this user.");

            var context = new PackageValidationContext
            {
                User = fullUser,
                NewFileSize = newFileSize,
                PackagePlan = fullUser.PackagePlan
            };

            var uploadCountHandler = new UploadCountLimitHandler(_context);
            var storageLimitHandler = new StorageLimitHandler(_context);

            uploadCountHandler.SetNext(storageLimitHandler);

            return await uploadCountHandler.HandleAsync(context);
        }

        public async Task<PackageLimitResult> CanDownloadAsync(ApplicationUser user)
        {
            var fullUser = await _userManager.Users
                .Include(u => u.PackagePlan)
                .FirstOrDefaultAsync(u => u.Id == user.Id);

            if (fullUser == null || fullUser.PackagePlan == null)
                return PackageLimitResult.Denied("No package is assigned to this user.");

            var context = new PackageValidationContext
            {
                User = fullUser,
                NewFileSize = 0,
                PackagePlan = fullUser.PackagePlan
            };

            var downloadCountHandler = new DownloadCountLimitHandler(_context);

            return await downloadCountHandler.HandleAsync(context);
        }
    }
}