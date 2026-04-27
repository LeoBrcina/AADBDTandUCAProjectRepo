using Microsoft.EntityFrameworkCore;
using PicGramWebApp.Data;

namespace PicGramWebApp.Services.Packages.Validation
{
    public class StorageLimitHandler : PackageValidationHandlerBase
    {
        private readonly ApplicationDbContext _context;

        public StorageLimitHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public override async Task<PackageLimitResult> HandleAsync(PackageValidationContext context)
        {
            var usedStorageBytes = await _context.Photos
                .Where(p => p.UserId == context.User.Id)
                .Select(p => (long?)p.FileSize)
                .SumAsync() ?? 0;

            if (usedStorageBytes + context.NewFileSize > context.PackagePlan.MaxStorageBytes)
            {
                return PackageLimitResult.Denied("Uploading this file would exceed your package storage limit.");
            }

            return await base.HandleAsync(context);
        }
    }
}