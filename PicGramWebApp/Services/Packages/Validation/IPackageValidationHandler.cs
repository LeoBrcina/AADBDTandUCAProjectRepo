namespace PicGramWebApp.Services.Packages.Validation
{
    // Chain of Responsibility pattern: defines a contract for validation handlers
    // that process a request sequentially, allowing each handler to either handle
    // the request or pass it to the next handler in the chain.
    public interface IPackageValidationHandler
    {
        IPackageValidationHandler SetNext(IPackageValidationHandler next);
        Task<PackageLimitResult> HandleAsync(PackageValidationContext context);
    }
}