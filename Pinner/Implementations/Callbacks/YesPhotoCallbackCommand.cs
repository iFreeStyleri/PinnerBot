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

        public override async Task ExecuteAsync(Update update, CancellationToken token)
        {
            var callback = update.CallbackQuery;
            
            await _client.SendPhoto(-1002312040693, InputFile.FromUri(callback.Message.Caption));
        }
    }
}
