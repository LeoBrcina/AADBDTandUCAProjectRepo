using Microsoft.Extensions.Configuration;

namespace PicGramWebApp.Services.Storage
{
    // Factory Method pattern: encapsulates the creation logic of storage providers,
    // allowing the application to switch between implementations (e.g., Local, Cloud)
    // without changing the consuming code.
    public class StorageProviderFactory
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;

        public StorageProviderFactory(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _configuration = configuration;
            _serviceProvider = serviceProvider;
        }

        public IStorageProvider Create()
        {
            var provider = _configuration["Storage:Provider"] ?? "Local";

            return provider switch
            {
                "Local" => _serviceProvider.GetRequiredService<LocalStorageProvider>(),
                _ => throw new NotSupportedException($"Storage provider '{provider}' is not supported.")
            };
        }
    }
}