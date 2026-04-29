using Microsoft.AspNetCore.Mvc.Filters;
using PicGramWebApp.Services.Metrics;

namespace PicGramWebApp.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class ActionCounterAspectAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var metrics = context.HttpContext.RequestServices.GetRequiredService<AppMetrics>();

            var controllerName = context.RouteData.Values["controller"]?.ToString() ?? "UnknownController";
            var actionName = context.RouteData.Values["action"]?.ToString() ?? "UnknownAction";
            var metricKey = $"{controllerName}.{actionName}";

            metrics.IncrementAction(metricKey);

            await next();
        }
    }
}