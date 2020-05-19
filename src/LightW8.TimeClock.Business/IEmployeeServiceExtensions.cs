using Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

#if !DEBUG
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
#endif

namespace LightW8.TimeClock.Business
{
    public static class IEmployeeServiceExtensions
    {
        public static IServiceCollection AddFakeEmployeeServiceClient(this IServiceCollection services)
        {
            services.AddSingleton<IEmployeeService, FakeEmployeeService>();
            return services;
        }

        public static IServiceCollection AddCosmosEmployeeServiceClient(this IServiceCollection services)
        {
            var companyName = "NewCo";
#if DEBUG
            // local emulator DB connection string (instructions here: https://docs.microsoft.com/en-us/azure/storage/common/storage-use-emulator)
            //var connectionString = @"";
            // cloud dev DB connectgion string
            var connectionString = @"";
#else
            var secretsUrl = "";
            var tokenProvider = new AzureServiceTokenProvider();
            var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(tokenProvider.KeyVaultTokenCallback));
            var credentail = keyVaultClient.GetSecretAsync(secretsUrl).Result;
            var connectionString = credentail.Value.ToString();
#endif
            //services.AddSingleton(new CosmosClient(EndpointUrl, authKey));
            //services.AddTransient(provider => provider.ResolveWith<CosmosEmployeeService>(companyName));
            //services.AddSingleton<IEmployeeService, CosmosEmployeeService>();
            services.AddSingleton<IEmployeeService>(new CosmosEmployeeService(new CosmosClient(connectionString), companyName));
            return services;
        }

        // Helper method to shorten code registration code. See here: https://stackoverflow.com/a/53885374/22528
        private static T ResolveWith<T>(this IServiceProvider provider, params object[] parameters) where T : class =>
            ActivatorUtilities.CreateInstance<T>(provider, parameters);
    }
}