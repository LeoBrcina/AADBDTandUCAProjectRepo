using PicGramWebApp.Data;
using PicGramWebApp.Models;

namespace PicGramWebApp.Services.Observers
{
    // Concrete observer: reacts to application action events by persisting them
    // as ActionLog entries in the database.
    public class ActionLogObserver : IPhotoActionObserver
    {
        private readonly ApplicationDbContext _context;

        public ActionLogObserver(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task UpdateAsync(PhotoActionEvent actionEvent)
        {
            var log = new ActionLog
            {
                UserId = actionEvent.UserId,
                ActionType = actionEvent.ActionType,
                Details = actionEvent.Details
            };

            _context.ActionLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}