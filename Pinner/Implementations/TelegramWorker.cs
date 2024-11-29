using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Pinner.Abstractions;
using Pinner.Abstractions.Services;
using Pinner.Common;
using Pinner.Implementations.Callbacks;
using Pinner.Implementations.Commands;
using Pinner.Implementations.Utils;
using Python.Runtime;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Pinner.Implementations
{
    public class TelegramWorker : ITelegramWorker
    {
        private readonly IConfiguration _configuration;
        private readonly IFinderService _searchPhotoService;
        private readonly Dictionary<string, CommandCallback> _callbacks = new();
        private readonly Dictionary<string, TextCommand> _commands = new();
        public ITelegramBotClient Client { get; init; }
        public IReadOnlyDictionary<string, CommandCallback> Callbacks => _callbacks;
        public IReadOnlyDictionary<string, TextCommand> Commands => _commands;
        
        public TelegramWorker(ITelegramBotClient client, IConfiguration config, IFinderService service)
        {
            Client = client;
            _ = Client.ReceiveAsync(Handler, ErrorHandler);
            Runtime.PythonDLL = "python312.dll";
            _configuration = config;
            _searchPhotoService = service;
            foreach (var timeString in config.GetSection("Times").Get<List<string>>())
            {
                var times = timeString.Split(':').Select(s => int.Parse(s)).ToArray();
                TaskScheduler.Instance.ScheduleTask(times[0], times[1], 24, SendAutoImage);
            }
            ConfigureCallbacks();
            ConfigureCommands();

        }

        private Task ErrorHandler(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        private async Task Handler(ITelegramBotClient client, Update update, CancellationToken token)
        {
            switch (update.Type)
            {
                case UpdateType.Message:
                {
                    try
                    {
                        if (update.Message != null && update.Message.Text != null)
                        {
                            var command = Commands.FirstOrDefault(s => update.Message.Text.Contains(s.Key));
                            if (command.Value != null)
                                await command.Value.ExecuteAsync(update, token);
                        }

                    }
                    catch(Exception ex)
                    {
                        if (update.Message.Text.FirstOrDefault() == '/')
                        {
                            await Client.SendMessage(update.Message.Chat, $"{ex.Message}", replyParameters: update.Message.MessageId);
                        }
                    }
                    break;
                }
                case UpdateType.CallbackQuery:
                {
                    try
                    {
                        var result = _callbacks.GetValueOrDefault(update.CallbackQuery.Data);
                        if (result != null)
                            await result.ExecuteAsync(update, token);
                    }
                    catch
                    {

                    }
                    break;
                }
            }
        }


        private void ConfigureCallbacks()
        {
            _callbacks.Add("Yes Photo", new YesPhotoCallbackCommand("Yes Photo", Client, _configuration));
            _callbacks.Add("No Photo", new NoPhotoCallbackCommand("No Photo", Client));
        }

        private void ConfigureCommands()
        {
            _commands.Add("/search", new SearchPhotoCommand("/search", Client, _configuration));
            _commands.Add("/search@SkyEllegentBot", new SearchPhotoCommand("/search@SkyEllegentBot", Client, _configuration));
        }

        public async Task Echo(CancellationToken token)
        {
            await Task.Delay(-1, token);
        }

        public async void SendAutoImage()
        {
            await _searchPhotoService.FindPhoto();
        }

    }
}
