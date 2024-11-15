using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pinner.Common;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Pinner.Implementations.Callbacks
{
    public class YesPhotoCallbackCommand : CommandCallback
    {
        public YesPhotoCallbackCommand(string data, ITelegramBotClient client) : base(data, client)
        {

        }

        public override Task ExecuteAsync(Update update, CancellationToken token)
        {
            //_client.SendPhoto(update.CallbackQuery.Message.Chat, )
            return Task.CompletedTask;
        }
    }
}
