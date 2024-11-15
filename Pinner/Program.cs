using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pinner.Abstractions;
using Pinner.Implementations;
using Telegram.Bot;
using Python.Runtime;

namespace Pinner
{
    public static class Program
    {
        public static ITelegramBotClient Client { get; private set; }
        public static async Task Main(string[] args)
        {
            var token = new CancellationToken();
            var builder = WebApplication.CreateBuilder();

            builder.Configuration.AddJsonFile("appsettings.json");
            var config = builder.Configuration;
            builder.Services.Configuring(config);

            var app = builder.Build();
            var worker = app.Services.GetRequiredService<ITelegramWorker>();
            await worker.Echo(token);
            await app.RunAsync();
        }


        private static IServiceCollection Configuring(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<ITelegramWorker, TelegramWorker>();
            services.AddSingleton(services => new TelegramBotClientOptions(configuration.GetSection("TelegramToken").Value));
            return services;
        }
    }
}