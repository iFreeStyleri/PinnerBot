using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Pinner.Abstractions;
using Pinner.Common;
using Pinner.Implementations.Callbacks;
using Pinner.Implementations.Commands;
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
        private readonly Dictionary<string, CommandCallback> _callbacks = new();
        private readonly Dictionary<string, TextCommand> _commands = new();
        public ITelegramBotClient Client { get; init; }
        public IReadOnlyDictionary<string, CommandCallback> Callbacks => _callbacks;
        public IReadOnlyDictionary<string, TextCommand> Commands => _commands;
        
        public TelegramWorker(TelegramBotClientOptions opt, IConfiguration config)
        {
            Client = new TelegramBotClient(opt);
            _ = Client.ReceiveAsync(Handler, ErrorHandler);
            Runtime.PythonDLL = "python312.dll";
            _configuration = config;

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
                    catch
                    {
                        if (update.Message.Text.FirstOrDefault() == '/')
                        {
                            await Client.SendMessage(update.Message.Chat, "Команда не найдена", replyParameters: update.Message.MessageId);
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
            _callbacks.Add("Yes Photo", new YesPhotoCallbackCommand("Yes Photo", Client));
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
    }
}
