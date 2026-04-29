using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PicGramWebApp.Data;
using PicGramWebApp.Services.Metrics;

namespace PicGramWebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MonitoringController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly AppMetrics _appMetrics;

        public MonitoringController(ApplicationDbContext context, AppMetrics appMetrics)
        {
            _context = context;
            _appMetrics = appMetrics;
        }

        [HttpGet]
        public IActionResult Health()
        {
            var result = new
            {
                status = "Healthy",
                timestamp = DateTime.UtcNow,
                databaseConfigured = _context.Database != null
            };

            return Json(result);
        }

        [HttpGet]
        public IActionResult Metrics()
        {
            var result = new
            {
                timestamp = DateTime.UtcNow,
                totals = new
                {
                    users = _context.Users.Count(),
                    photos = _context.Photos.Count(),
                    actionLogs = _context.ActionLogs.Count(),
                    packageChangeRequests = _context.PackageChangeRequests.Count()
                },
                actionCounts = _appMetrics.GetAllActionCounts(),
                averageExecutionTimes = _appMetrics.GetAllAverageExecutionTimes()
            };

            return Json(result);
        }
    }
}