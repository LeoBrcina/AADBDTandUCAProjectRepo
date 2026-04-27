using Microsoft.EntityFrameworkCore;
using PicGramWebApp.Data;
using PicGramWebApp.Models;

namespace PicGramWebApp.Services.Search
{
    public class PhotoSearchService : IPhotoSearchService
    {
        private readonly ApplicationDbContext _context;

        public PhotoSearchService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Photo> Search(PhotoSearchCriteria criteria)
        {
            var query = _context.Photos
                .Include(p => p.User)
                .Include(p => p.PhotoHashtags)
                    .ThenInclude(ph => ph.Hashtag)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(criteria.Hashtag))
            {
                var normalizedHashtag = criteria.Hashtag.Trim().TrimStart('#').ToLower();
                query = query.Where(p => p.PhotoHashtags.Any(ph => ph.Hashtag!.Name.ToLower().Contains(normalizedHashtag)));
            }

            if (!string.IsNullOrWhiteSpace(criteria.Author))
            {
                var normalizedAuthor = criteria.Author.Trim().ToLower();
                query = query.Where(p => p.User!.Email!.ToLower().Contains(normalizedAuthor));
            }

            if (criteria.FromDate.HasValue)
            {
                query = query.Where(p => p.UploadedAt >= criteria.FromDate.Value);
            }

            if (criteria.ToDate.HasValue)
            {
                var inclusiveToDate = criteria.ToDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(p => p.UploadedAt <= inclusiveToDate);
            }

            if (criteria.MinSize.HasValue)
            {
                query = query.Where(p => p.FileSize >= criteria.MinSize.Value);
            }

            if (criteria.MaxSize.HasValue)
            {
                query = query.Where(p => p.FileSize <= criteria.MaxSize.Value);
            }

            return query
                .OrderByDescending(p => p.UploadedAt)
                .ToList();
        }
    }
}