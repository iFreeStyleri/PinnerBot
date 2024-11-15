using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pinner.Abstractions;
using Pinner.Common;
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
        private readonly Dictionary<string, CommandCallback> _callbacks = new();
        private readonly Dictionary<string, TextCommand> _commands = new();
        public ITelegramBotClient Client { get; init; }
        public IReadOnlyDictionary<string, CommandCallback> Callbacks => _callbacks;
        public IReadOnlyDictionary<string, TextCommand> Commands => _commands;

        public TelegramWorker(TelegramBotClientOptions opt)
        {
            Client = new TelegramBotClient(opt);
            ConfigureCallbacks();
            ConfigureCommands();
            _ = Client.ReceiveAsync(Handler, ErrorHandler);
            Runtime.PythonDLL = "python312.dll";
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
                        var command = Commands.First(s => update.Message.Text.Contains(s.Key));
                        await command.Value.ExecuteAsync(update, token);

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
            }
        }


        private void ConfigureCallbacks()
        {

        }

        private void ConfigureCommands()
        {
            _commands.Add("/search", new SearchPhotoCommand("/search", Client));
            _commands.Add("/search@SkyEllegentBot", new SearchPhotoCommand("/search@SkyEllegentBot", Client));
        }

        public async Task Echo(CancellationToken token)
        {
            await Task.Delay(-1, token);
        }
    }
}
