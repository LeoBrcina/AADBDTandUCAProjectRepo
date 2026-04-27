namespace PicGramWebApp.Services.Packages.Validation
{
    // Base handler for Chain of Responsibility: provides default chaining behavior,
    // forwarding the request to the next handler if the current one does not block it.
    public abstract class PackageValidationHandlerBase : IPackageValidationHandler
    {
        private IPackageValidationHandler? _next;

        public IPackageValidationHandler SetNext(IPackageValidationHandler next)
        {
            _next = next;
            return next;
        }

        public virtual async Task<PackageLimitResult> HandleAsync(PackageValidationContext context)
        {
            if (_next != null)
            {
                return await _next.HandleAsync(context);
            }

            return PackageLimitResult.Allowed();
        }
    }
}