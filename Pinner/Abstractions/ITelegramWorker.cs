using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pinner.Common;
using Telegram.Bot;

namespace Pinner.Abstractions
{
    public interface ITelegramWorker
    {
        ITelegramBotClient Client { get; }
        IReadOnlyDictionary<string, CommandCallback> Callbacks { get; } 
        IReadOnlyDictionary<string, TextCommand> Commands { get; }

        Task Echo(CancellationToken token);
    }
}
