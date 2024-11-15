using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Pinner.Common
{
    public abstract class TextCommand
    {
        protected readonly ITelegramBotClient _client;
        public string Data { get; set; }
        protected TextCommand(string data, ITelegramBotClient client)
        {
            Data = data;
            _client = client;
        }

        public abstract Task ExecuteAsync(Update update, CancellationToken token);
    }
}
