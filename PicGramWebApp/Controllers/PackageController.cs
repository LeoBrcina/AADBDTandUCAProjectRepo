using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PicGramWebApp.Data;
using PicGramWebApp.Models;
using PicGramWebApp.Models.ViewModels;
using PicGramWebApp.Services.Commands;
using PicGramWebApp.Services.Logging;
using PicGramWebApp.Services.Packages;

namespace PicGramWebApp.Controllers
{
    [Authorize]
    public class PackageController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly PackageChangeService _packageChangeService;
        private readonly AppActionLogger _appActionLogger;

        public PackageController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            PackageChangeService packageChangeService,
            AppActionLogger appActionLogger)
        {
            _context = context;
            _userManager = userManager;
            _packageChangeService = packageChangeService;
            _appActionLogger = appActionLogger;
        }

        public async Task<IActionResult> Usage()
        {
            var user = await _userManager.Users
                .Include(u => u.PackagePlan)
                .FirstOrDefaultAsync(u => u.Id == _userManager.GetUserId(User));

            if (user == null)
            {
                return NotFound();
            }

            await _packageChangeService.ApplyPendingChangesAsync(user);

            user = await _userManager.Users
                .Include(u => u.PackagePlan)
                .FirstOrDefaultAsync(u => u.Id == user.Id);

            var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            var uploadedPhotos = _context.Photos.Count(p =>
                p.UserId == user.Id &&
                p.UploadedAt >= startOfMonth);

            var usedStorageBytes = _context.Photos
                .Where(p => p.UserId == user.Id)
                .Select(p => (long?)p.FileSize)
                .Sum() ?? 0;

            var downloadCount = _context.ActionLogs.Count(l =>
                l.UserId == user.Id &&
                l.CreatedAt >= startOfMonth &&
                (l.ActionType == "DownloadOriginal" || l.ActionType == "DownloadProcessed"));

            var model = new PackageUsageViewModel
            {
                Email = user.Email ?? "",
                PackageName = user.PackagePlan?.Name ?? "None",
                PackagePrice = user.PackagePlan?.Price ?? 0,
                UploadedPhotos = uploadedPhotos,
                UsedStorageBytes = usedStorageBytes,
                DownloadCount = downloadCount,
                MaxUploadsPerMonth = user.PackagePlan?.MaxUploadsPerMonth ?? 0,
                MaxStorageBytes = user.PackagePlan?.MaxStorageBytes ?? 0,
                MaxDownloadsPerMonth = user.PackagePlan?.MaxDownloadsPerMonth ?? 0
            };

            return View(model);
        }

        public async Task<IActionResult> Change()
        {
            var user = await _userManager.Users
                .Include(u => u.PackagePlan)
                .FirstOrDefaultAsync(u => u.Id == _userManager.GetUserId(User));

            if (user == null || user.PackagePlan == null)
            {
                return NotFound();
            }

            await _packageChangeService.ApplyPendingChangesAsync(user);

            user = await _userManager.Users
                .Include(u => u.PackagePlan)
                .FirstOrDefaultAsync(u => u.Id == user.Id);

            var model = new PackageChangeViewModel
            {
                CurrentPackageName = user.PackagePlan?.Name ?? "None",
                AvailablePackages = _context.PackagePlans
                    .Select(p => new SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = p.Name
                    })
                    .ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Change(PackageChangeViewModel model)
        {
            var user = await _userManager.Users
                .Include(u => u.PackagePlan)
                .FirstOrDefaultAsync(u => u.Id == _userManager.GetUserId(User));

            if (user == null || user.PackagePlan == null)
            {
                return NotFound();
            }

            var command = new RequestPackageChangeCommand(
                _context,
                _userManager,
                _appActionLogger,
                user,
                model.RequestedPackagePlanId);

            var result = await command.ExecuteAsync();

            if (!result.Success)
            {
                model.CurrentPackageName = user.PackagePlan.Name;
                model.AvailablePackages = _context.PackagePlans
                    .Select(p => new SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = p.Name
                    })
                    .ToList();

                model.Message = result.ErrorMessage;
                return View(model);
            }

            TempData["SuccessMessage"] = "Package change request saved. It will become active tomorrow.";
            return RedirectToAction("Usage");
        }
    }
}