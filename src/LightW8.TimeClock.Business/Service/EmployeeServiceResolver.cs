using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace LightW8.TimeClock.Business.Service
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddEmployeeService(this IServiceCollection services, IConfiguration configuration)
        {
            var section = configuration.GetSection("DBType").Value;
            services.Configure<DBOptions>(configuration.GetSection(section));

            services.AddSingleton<CompanyNameResolver>();
            services.AddSingleton<ConnectionStringResolver>();
            services.AddSingleton<EmployeeServiceResolver>();

            services.AddSingleton<FakeEmployeeService>();
            services.AddSingleton<CosmosEmployeeService>();

            services.AddSingleton<IEmployeeService>(serviceProvider => serviceProvider.GetService<EmployeeServiceResolver>().GetEmployeeService());

            return services;
        }

        // Helper method to shorten code registration code. See here: https://stackoverflow.com/a/53885374/22528
        private static T ResolveWith<T>(this IServiceProvider provider, params object[] parameters) where T : class => ActivatorUtilities.CreateInstance<T>(provider, parameters);
    }

    public class DBOptions
    {
        public string Type { get; set; }
        public string SecretsUrl { get; set; }
        public string Connection { get; set; }
        public string CompanyName { get; set; }
    }

    public class EmployeeServiceResolver
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IOptions<DBOptions> _appSettings;

        public EmployeeServiceResolver(IServiceProvider serviceProvider, IOptions<DBOptions> appSettings)
        {
            _serviceProvider = serviceProvider;
            _appSettings = appSettings;
        }

        public IEmployeeService GetEmployeeService() => _appSettings.Value.Type switch
        {
            "Cosmos" => _serviceProvider.GetService<CosmosEmployeeService>(),
            "Fake" => _serviceProvider.GetService<FakeEmployeeService>(),
            _ => _serviceProvider.GetService<FakeEmployeeService>(),
        };
    }

    public class CompanyNameResolver
    {
        private readonly IOptions<DBOptions> _appSettings;

        public CompanyNameResolver(IOptions<DBOptions> appSettings)
        {
            _appSettings = appSettings;
        }

        public string GetCompanyName() => _appSettings.Value.CompanyName;
    }

    public class ConnectionStringResolver
    {
        private readonly IOptions<DBOptions> _appSettings;

        public ConnectionStringResolver(IOptions<DBOptions> appSettings)
        {
            _appSettings = appSettings;
        }

        public string GetConnectionString()
        {
            if(_appSettings.Value.SecretsUrl == null)
                return _appSettings.Value.Connection;

            var secretsUrl = _appSettings.Value.SecretsUrl;
            var tokenProvider = new AzureServiceTokenProvider();
            var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(tokenProvider.KeyVaultTokenCallback));
            var credentail = keyVaultClient.GetSecretAsync(secretsUrl).Result;
            var connectionString = credentail.Value.ToString();
            return connectionString;
        }
    }
}