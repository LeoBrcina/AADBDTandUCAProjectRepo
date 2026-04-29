using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PicGramWebApp.Data;
using PicGramWebApp.Filters;
using PicGramWebApp.Models;
using PicGramWebApp.Services.Facade;
using PicGramWebApp.Services.ImageProcessing;
using PicGramWebApp.Services.Logging;
using PicGramWebApp.Services.Packages;
using PicGramWebApp.Services.Search;
using PicGramWebApp.Services.Storage;
using SixLabors.ImageSharp;

namespace PicGramWebApp.Controllers
{
    [Authorize]
    public class PhotoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly PhotoFacade _photoFacade;
        private readonly StorageProviderFactory _storageProviderFactory;
        private readonly PackageLimitService _packageLimitService;
        private readonly IPhotoSearchService _photoSearchService;
        private readonly AppActionLogger _appActionLogger;

        public PhotoController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            PhotoFacade photoFacade,
            StorageProviderFactory storageProviderFactory,
            PackageLimitService packageLimitService,
            IPhotoSearchService photoSearchService,
            AppActionLogger appActionLogger)
        {
            _context = context;
            _userManager = userManager;
            _photoFacade = photoFacade;
            _storageProviderFactory = storageProviderFactory;
            _packageLimitService = packageLimitService;
            _photoSearchService = photoSearchService;
            _appActionLogger = appActionLogger;
        }

        [AllowAnonymous]
        [ExecutionTimeAspect]
        [ActionCounterAspect]
        public IActionResult Search(string hashtag, string author, DateTime? fromDate, DateTime? toDate, long? minSize, long? maxSize)
        {
            var criteria = new PhotoSearchCriteriaBuilder()
                .WithHashtag(hashtag)
                .WithAuthor(author)
                .WithFromDate(fromDate)
                .WithToDate(toDate)
                .WithMinSize(minSize)
                .WithMaxSize(maxSize)
                .Build();

            var photos = _photoSearchService.Search(criteria);

            return View(photos);
        }

        [ExecutionTimeAspect]
        [ActionCounterAspect]
        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file, string description, string hashtags)
        {
            var photo = await _photoFacade.UploadPhotoAsync(User, file, description, hashtags);

            if (photo == null)
            {
                ViewBag.ErrorMessage = "Upload failed. You may have reached your package limits or submitted an invalid file.";
                return View();
            }

            return RedirectToAction("MyPhotos");
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            var photos = _context.Photos
                .Include(p => p.User)
                .Include(p => p.PhotoHashtags)
                    .ThenInclude(ph => ph.Hashtag)
                .OrderByDescending(p => p.UploadedAt)
                .Take(10)
                .ToList();

            return View(photos);
        }

        [AllowAnonymous]
        public IActionResult Details(int id)
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

            return View(photo);
        }

        [Authorize]
        [ExecutionTimeAspect]
        [ActionCounterAspect]
        public async Task<IActionResult> Download(int id)
        {
            var photo = _context.Photos.FirstOrDefault(p => p.Id == id);

            if (photo == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            var downloadCheck = await _packageLimitService.CanDownloadAsync(currentUser);
            if (!downloadCheck.IsAllowed)
            {
                ViewBag.ErrorMessage = downloadCheck.ErrorMessage ?? "Download limit reached.";
                return View("~/Views/Shared/ErrorMessage.cshtml");
            }

            var storageProvider = _storageProviderFactory.Create();
            var fullPath = storageProvider.GetFullPath(photo.FilePath);

            if (!System.IO.File.Exists(fullPath))
            {
                return NotFound();
            }

            await LogAction("DownloadOriginal", $"PhotoId={photo.Id}, FileName={photo.FileName}");

            var contentType = "application/octet-stream";
            return PhysicalFile(fullPath, contentType, photo.FileName);
        }

        [Authorize]
        [ExecutionTimeAspect]
        [ActionCounterAspect]
        public async Task<IActionResult> DownloadProcessed(
            int id,
            int? width,
            int? height,
            string format,
            bool applySepia,
            bool applyBlur,
            bool applyGrayscale,
            float? sharpenAmount,
            float? brightness,
            float? contrast)
        {
            var photo = _context.Photos.FirstOrDefault(p => p.Id == id);

            if (photo == null)
                return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            var downloadCheck = await _packageLimitService.CanDownloadAsync(currentUser);
            if (!downloadCheck.IsAllowed)
            {
                ViewBag.ErrorMessage = downloadCheck.ErrorMessage ?? "Download limit reached.";
                return View("~/Views/Shared/ErrorMessage.cshtml");
            }

            var storageProvider = _storageProviderFactory.Create();
            var fullPath = storageProvider.GetFullPath(photo.FilePath);

            if (!System.IO.File.Exists(fullPath))
                return NotFound();

            using var image = await Image.LoadAsync(fullPath);

            var strategies = new List<IImageProcessingStrategy>();

            if (width.HasValue && height.HasValue)
                strategies.Add(new ResizeStrategy(width.Value, height.Value));

            if (applySepia)
                strategies.Add(new SepiaStrategy());

            if (applyBlur)
                strategies.Add(new BlurStrategy());

            if (applyGrayscale)
                strategies.Add(new GrayscaleStrategy());

            if (sharpenAmount.HasValue && sharpenAmount.Value > 0)
                strategies.Add(new SharpenStrategy(sharpenAmount.Value));

            if (brightness.HasValue && brightness.Value > 0)
                strategies.Add(new BrightnessStrategy(brightness.Value));

            if (contrast.HasValue && contrast.Value > 0)
                strategies.Add(new ContrastStrategy(contrast.Value));

            foreach (var strategy in strategies)
            {
                strategy.Apply(image);
            }

            var ms = new MemoryStream();

            switch (format?.ToLower())
            {
                case "png":
                    await image.SaveAsPngAsync(ms);
                    break;
                case "bmp":
                    await image.SaveAsBmpAsync(ms);
                    break;
                default:
                    await image.SaveAsJpegAsync(ms);
                    format = "jpg";
                    break;
            }

            ms.Position = 0;

            await LogAction("DownloadProcessed", $"PhotoId={photo.Id}, Format={format}");

            var contentType = format?.ToLower() switch
            {
                "png" => "image/png",
                "bmp" => "image/bmp",
                _ => "image/jpeg"
            };

            return File(ms, contentType, $"processed_{Path.GetFileNameWithoutExtension(photo.FileName)}.{format}");
        }

        [AllowAnonymous]
        public IActionResult DownloadOptions(int id)
        {
            var photo = _context.Photos.FirstOrDefault(p => p.Id == id);

            if (photo == null)
            {
                return NotFound();
            }

            return View(photo);
        }

        [Authorize]
        public IActionResult Edit(int id)
        {
            var userId = _userManager.GetUserId(User);

            var photo = _context.Photos
                .Include(p => p.PhotoHashtags)
                    .ThenInclude(ph => ph.Hashtag)
                .FirstOrDefault(p => p.Id == id && p.UserId == userId);

            if (photo == null)
            {
                return NotFound();
            }

            ViewBag.Hashtags = string.Join(", ", photo.PhotoHashtags!.Select(ph => ph.Hashtag!.Name));

            return View(photo);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(int id, string description, string hashtags)
        {
            var success = await _photoFacade.EditPhotoAsync(User, id, description, hashtags);

            if (!success)
                return NotFound();

            return RedirectToAction("MyPhotos");
        }

        public IActionResult MyPhotos()
        {
            var userId = _userManager.GetUserId(User);

            var photos = _context.Photos
                .Where(p => p.UserId == userId)
                .Include(p => p.PhotoHashtags)
                    .ThenInclude(ph => ph.Hashtag)
                .ToList();

            return View(photos);
        }

        private async Task LogAction(string actionType, string? details = null)
        {
            await _appActionLogger.LogAsync(actionType, details);
        }
    }
}