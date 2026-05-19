using Microsoft.EntityFrameworkCore;
using PicGramWebApp.Data;
using PicGramWebApp.Models;
using PicGramWebApp.Services.Functional;

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

            var normalizedHashtag = PhotoFunctionalHelpers.NormalizeHashtag(criteria.Hashtag);

            if (!string.IsNullOrWhiteSpace(normalizedHashtag))
            {
                query = query.Where(p =>
                    p.PhotoHashtags.Any(ph =>
                        ph.Hashtag!.Name.ToLower().Contains(normalizedHashtag)));
            }

            var normalizedAuthor = PhotoFunctionalHelpers.NormalizeAuthor(criteria.Author);

            if (!string.IsNullOrWhiteSpace(normalizedAuthor))
            {
                var authorSearch = normalizedAuthor.ToLowerInvariant();

                query = query.Where(p =>
                    p.User!.Email!.ToLower().Contains(authorSearch));
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