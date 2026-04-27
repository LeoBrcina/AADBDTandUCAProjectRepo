using Microsoft.EntityFrameworkCore;
using PicGramWebApp.Data;
using PicGramWebApp.Services.Logging;

namespace PicGramWebApp.Services.Commands
{
    public class AdminDeletePhotoCommand : ICommand<AdminDeletePhotoResult>
    {
        private readonly ApplicationDbContext _context;
        private readonly AppActionLogger _appActionLogger;
        private readonly string _contentRootPath;
        private readonly int _photoId;

        public AdminDeletePhotoCommand(
            ApplicationDbContext context,
            AppActionLogger appActionLogger,
            string contentRootPath,
            int photoId)
        {
            _context = context;
            _appActionLogger = appActionLogger;
            _contentRootPath = contentRootPath;
            _photoId = photoId;
        }

        public async Task<AdminDeletePhotoResult> ExecuteAsync()
        {
            var photo = await _context.Photos
                .Include(p => p.PhotoHashtags)
                .FirstOrDefaultAsync(p => p.Id == _photoId);

            if (photo == null)
            {
                return AdminDeletePhotoResult.Fail("Photo not found.");
            }

            await _appActionLogger.LogAsync(
                "AdminDeletedPhoto",
                $"Deleted PhotoId={photo.Id}");

            var filePath = Path.Combine(
                _contentRootPath,
                "wwwroot",
                photo.FilePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));

            var photoHashtags = _context.PhotoHashtags
                .Where(ph => ph.PhotoId == photo.Id)
                .ToList();

            _context.PhotoHashtags.RemoveRange(photoHashtags);
            _context.Photos.Remove(photo);

            await _context.SaveChangesAsync();

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            return AdminDeletePhotoResult.Ok();
        }
    }
}