using Microsoft.EntityFrameworkCore;
using PicGramWebApp.Data;
using PicGramWebApp.Models;
using PicGramWebApp.Services.Logging;

namespace PicGramWebApp.Services.Commands
{
    // Command pattern: encapsulates the admin photo editing operation,
    // including photo updates, hashtag synchronization, and logging.
    public class AdminEditPhotoCommand : ICommand<AdminEditPhotoResult>
    {
        private readonly ApplicationDbContext _context;
        private readonly AppActionLogger _appActionLogger;
        private readonly int _photoId;
        private readonly string _description;
        private readonly string _hashtags;

        public AdminEditPhotoCommand(
            ApplicationDbContext context,
            AppActionLogger appActionLogger,
            int photoId,
            string description,
            string hashtags)
        {
            _context = context;
            _appActionLogger = appActionLogger;
            _photoId = photoId;
            _description = description;
            _hashtags = hashtags;
        }

        public async Task<AdminEditPhotoResult> ExecuteAsync()
        {
            var photo = await _context.Photos
                .Include(p => p.PhotoHashtags)
                .FirstOrDefaultAsync(p => p.Id == _photoId);

            if (photo == null)
            {
                return AdminEditPhotoResult.Fail("Photo not found.");
            }

            photo.Description = _description;

            var existingLinks = _context.PhotoHashtags
                .Where(ph => ph.PhotoId == photo.Id)
                .ToList();

            _context.PhotoHashtags.RemoveRange(existingLinks);
            await _context.SaveChangesAsync();

            if (!string.IsNullOrWhiteSpace(_hashtags))
            {
                var hashtagList = _hashtags
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(h => h.Trim().TrimStart('#').ToLower())
                    .Distinct()
                    .ToList();

                foreach (var hashtagName in hashtagList)
                {
                    var existingHashtag = await _context.Hashtags
                        .FirstOrDefaultAsync(h => h.Name == hashtagName);

                    if (existingHashtag == null)
                    {
                        existingHashtag = new Hashtag { Name = hashtagName };
                        _context.Hashtags.Add(existingHashtag);
                        await _context.SaveChangesAsync();
                    }

                    _context.PhotoHashtags.Add(new PhotoHashtag
                    {
                        PhotoId = photo.Id,
                        HashtagId = existingHashtag.Id
                    });
                }
            }

            await _context.SaveChangesAsync();

            await _appActionLogger.LogAsync(
                "AdminEditedPhoto",
                $"Edited PhotoId={photo.Id}");

            return AdminEditPhotoResult.Ok();
        }
    }
}