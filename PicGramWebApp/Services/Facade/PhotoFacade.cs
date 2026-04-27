using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PicGramWebApp.Data;
using PicGramWebApp.Models;
using PicGramWebApp.Services.Observers;
using PicGramWebApp.Services.Packages;
using PicGramWebApp.Services.Storage;
using System.Security.Claims;

namespace PicGramWebApp.Services.Facade
{
    // Facade pattern: provides a simplified entry point for complex photo workflows
    // such as upload/edit, while hiding storage, hashtag, validation, and logging details.
    public class PhotoFacade
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly PhotoActionSubject _photoActionSubject;
        private readonly StorageProviderFactory _storageProviderFactory;
        private readonly PackageLimitService _packageLimitService;

        public PhotoFacade(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            PhotoActionSubject photoActionSubject,
            StorageProviderFactory storageProviderFactory,
            PackageLimitService packageLimitService)
        {
            _context = context;
            _userManager = userManager;
            _photoActionSubject = photoActionSubject;
            _storageProviderFactory = storageProviderFactory;
            _packageLimitService = packageLimitService;
        }

        public async Task<Photo?> UploadPhotoAsync(ClaimsPrincipal currentUser, IFormFile file, string description, string hashtags)
        {
            if (file == null || file.Length == 0)
                return null;

            var user = await _userManager.GetUserAsync(currentUser);
            if (user == null)
                return null;

            var uploadCheck = await _packageLimitService.CanUploadAsync(user, file.Length);
            if (!uploadCheck.IsAllowed)
            {
                return null;
            }

            var storageProvider = _storageProviderFactory.Create();
            var savedFile = await storageProvider.SaveFileAsync(file);

            var photo = new Photo
            {
                FileName = savedFile.FileName,
                FilePath = savedFile.FilePath,
                Description = description,
                FileSize = file.Length,
                UserId = user.Id
            };

            _context.Photos.Add(photo);
            await _context.SaveChangesAsync();

            await ApplyHashtagsAsync(photo.Id, hashtags);

            await NotifyAsync(user.Id, "UploadPhoto", $"PhotoId={photo.Id}, FileName={photo.FileName}");

            return photo;
        }

        public async Task<bool> EditPhotoAsync(ClaimsPrincipal currentUser, int photoId, string description, string hashtags)
        {
            var user = await _userManager.GetUserAsync(currentUser);
            if (user == null)
                return false;

            var photo = await _context.Photos
                .Include(p => p.PhotoHashtags)
                .FirstOrDefaultAsync(p => p.Id == photoId && p.UserId == user.Id);

            if (photo == null)
                return false;

            photo.Description = description;

            var existingLinks = _context.PhotoHashtags.Where(ph => ph.PhotoId == photo.Id).ToList();
            _context.PhotoHashtags.RemoveRange(existingLinks);
            await _context.SaveChangesAsync();

            await ApplyHashtagsAsync(photo.Id, hashtags);

            await NotifyAsync(user.Id, "EditPhoto", $"PhotoId={photo.Id}");

            return true;
        }

        private async Task ApplyHashtagsAsync(int photoId, string hashtags)
        {
            if (string.IsNullOrWhiteSpace(hashtags))
                return;

            var hashtagList = hashtags
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(h => h.Trim().TrimStart('#').ToLower())
                .Distinct()
                .ToList();

            foreach (var hashtagName in hashtagList)
            {
                var existingHashtag = await _context.Hashtags.FirstOrDefaultAsync(h => h.Name == hashtagName);

                if (existingHashtag == null)
                {
                    existingHashtag = new Hashtag { Name = hashtagName };
                    _context.Hashtags.Add(existingHashtag);
                    await _context.SaveChangesAsync();
                }

                _context.PhotoHashtags.Add(new PhotoHashtag
                {
                    PhotoId = photoId,
                    HashtagId = existingHashtag.Id
                });
            }

            await _context.SaveChangesAsync();
        }

        private async Task NotifyAsync(string? userId, string actionType, string? details)
        {
            var actionEvent = new PhotoActionEvent
            {
                UserId = userId,
                ActionType = actionType,
                Details = details
            };

            await _photoActionSubject.NotifyAsync(actionEvent);
        }
    }
}