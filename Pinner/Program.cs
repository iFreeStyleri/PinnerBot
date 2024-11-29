using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pinner.Abstractions;
using Pinner.Abstractions.Services;
using Pinner.DAL;
using Pinner.DAL.Entities;
using Pinner.DAL.Repository.Abstractions;
using Pinner.DAL.Repository.Implementations;
using Pinner.Implementations;
using Pinner.Implementations.Utils;
using Telegram.Bot;
using Python.Runtime;

namespace Pinner
{
    public static class Program
    {
        public static string AppPath => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static IServiceProvider Services { get; private set; }
        public static async Task Main(string[] args)
        {
            var token = new CancellationToken();
            var builder = WebApplication.CreateBuilder();

            builder.Configuration.AddJsonFile("appsettings.json");
            var config = builder.Configuration;
            builder.Services.Configuring(config);

            var app = builder.Build();
            var worker = app.Services.GetRequiredService<ITelegramWorker>();
            Services = app.Services;
            await worker.Echo(token);
            await app.RunAsync();
        }


        private static IServiceCollection Configuring(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<ITelegramWorker, TelegramWorker>();
            services.AddSingleton<ITelegramBotClient, TelegramBotClient>(services 
                => new TelegramBotClient(configuration.GetSection("TelegramToken").Value));
            services.AddTransient<IBaseRepository<Tag>, BaseRepository<Tag>>();
            services.AddTransient<IBaseRepository<Topic>, BaseRepository<Topic>>();
            services.AddTransient<IFinderService, SearchPhotoService>();
            services.AddDbContext<BotContext>(opt =>
            {
                var conString = configuration.GetSection("Psql").Value;
                opt.UseNpgsql(conString);
            }, ServiceLifetime.Transient, ServiceLifetime.Transient);
            return services;
        }
    }
}