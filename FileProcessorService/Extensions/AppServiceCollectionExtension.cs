using FullTextSearch.Core.Contracts;
using FullTextSearch.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FileProcessorService.Extensions
{
    public static class AppServiceCollectionExtension
    {
        /// <summary>
        /// Регистрира услугите на приложението в IoC контейнера
        /// </summary>
        /// <param name="services">Регистрирани услуги</param>
        /// <param name="Configuration">Настройки на приложението</param>
        public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddLogging();
            services.AddHostedService<ProcessorHostedService>();
            services.AddScoped<IConsoleTaskRecieverService, ConsoleTaskRecieverService>();
            services.AddScoped<IIndexService, IndexService>();
            services.AddScoped<IMongoCdnService, MongoCdnService>();
        }

        /// <summary>
        /// Регистрира контекстите на приложението в IoC контейнера
        /// </summary>
        /// <param name="services">Регистрирани услуги</param>
        /// <param name="Configuration">Настройки на приложението</param>
        public static void AddAppDbContext(this IServiceCollection services, IConfiguration Configuration)
        {
            
        }
    }
}
