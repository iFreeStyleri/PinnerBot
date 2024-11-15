using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Pinner.Common
{
    public abstract class CommandCallback
    {
        protected readonly ITelegramBotClient _client;
        public string Data { get; private set; }
        
        protected CommandCallback(string data, ITelegramBotClient client)
        {
            Data = data;
            _client = client;
        }
        public abstract Task ExecuteAsync(Update update, CancellationToken token);
    }
}
