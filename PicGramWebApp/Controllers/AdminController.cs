using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PicGramWebApp.Data;
using PicGramWebApp.Filters;
using PicGramWebApp.Models;
using PicGramWebApp.Models.ViewModels;
using PicGramWebApp.Services.Commands;
using PicGramWebApp.Services.Logging;

namespace PicGramWebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly AppActionLogger _appActionLogger;

        public AdminController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            AppActionLogger appActionLogger)
        {
            _userManager = userManager;
            _context = context;
            _appActionLogger = appActionLogger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Users()
        {
            var users = _userManager.Users
                .Include(u => u.PackagePlan)
                .ToList();

            return View(users);
        }

        public IActionResult Photos()
        {
            var photos = _context.Photos
                .Include(p => p.User)
                .Include(p => p.PhotoHashtags)
                    .ThenInclude(ph => ph.Hashtag)
                .OrderByDescending(p => p.UploadedAt)
                .ToList();

            return View(photos);
        }

        public IActionResult Logs()
        {
            var logs = _context.ActionLogs
                .Include(l => l.User)
                .OrderByDescending(l => l.CreatedAt)
                .ToList();

            return View(logs);
        }

        public IActionResult EditUserPackage(string id)
        {
            var user = _userManager.Users
                .Include(u => u.PackagePlan)
                .FirstOrDefault(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            ViewBag.PackagePlans = _context.PackagePlans
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Name,
                    Selected = user.PackagePlanId == p.Id
                })
                .ToList();

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUserPackage(string id, int packagePlanId)
        {
            var command = new AdminChangeUserPackageCommand(
                _context,
                _userManager,
                _appActionLogger,
                id,
                packagePlanId);

            var result = await command.ExecuteAsync();

            if (!result.Success)
            {
                return NotFound();
            }

            return RedirectToAction("Users");
        }

        public IActionResult EditPhoto(int id)
        {
            var photo = _context.Photos
                .Include(p => p.User)
                .Include(p => p.PhotoHashtags)
                    .ThenInclude(ph => ph.Hashtag)
                .FirstOrDefault(p => p.Id == id);

            if (photo == null)
            {
                return NotFound();
            }

            ViewBag.Hashtags = string.Join(", ", photo.PhotoHashtags!.Select(ph => ph.Hashtag!.Name));

            return View(photo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPhoto(int id, string description, string hashtags)
        {
            var command = new AdminEditPhotoCommand(
                _context,
                _appActionLogger,
                id,
                description,
                hashtags);

            var result = await command.ExecuteAsync();

            if (!result.Success)
            {
                return NotFound();
            }

            return RedirectToAction("Photos");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePhoto(int id)
        {
            var command = new AdminDeletePhotoCommand(
                _context,
                _appActionLogger,
                Directory.GetCurrentDirectory(),
                id);

            var result = await command.ExecuteAsync();

            if (!result.Success)
            {
                return NotFound();
            }

            return RedirectToAction("Photos");
        }

        [ExecutionTimeAspect]
        public IActionResult Statistics()
        {
            var users = _userManager.Users
                .Include(u => u.PackagePlan)
                .ToList();

            var stats = users.Select(user => new UserStatisticsViewModel
            {
                UserId = user.Id,
                Email = user.Email ?? "",
                PackageName = user.PackagePlan?.Name ?? "None",
                PhotoCount = _context.Photos.Count(p => p.UserId == user.Id),
                ActionCount = _context.ActionLogs.Count(l => l.UserId == user.Id),
                LastActionAt = _context.ActionLogs
                    .Where(l => l.UserId == user.Id)
                    .OrderByDescending(l => l.CreatedAt)
                    .Select(l => (DateTime?)l.CreatedAt)
                    .FirstOrDefault()
            }).ToList();

            return View(stats);
        }
    }
}