using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pinner.Common;
using Telegram.Bot;
using Telegram.Bot.Types;
using File = System.IO.File;

namespace Pinner.Implementations.Callbacks
{
    public class NoPhotoCallbackCommand : CommandCallback
    {
        public NoPhotoCallbackCommand(string data, ITelegramBotClient client) : base(data, client)
        {
        }

        public override async Task ExecuteAsync(Update update, CancellationToken token)
        {
            var callback = update.CallbackQuery;
            var query = callback.Message.Text.Split("\n").ToList();
            var album = new List<InputMediaPhoto>();
            var streams = new List<FileStream>();
            var files = query.Skip(1).Select(f => Program.AppPath + "/output/" + $"{query.First()}/" + f).ToList();
            files.ForEach(File.Delete);
            await _client.EditMessageText(callback.Message.Chat, callback.Message.MessageId, "\u274c\u274c\u274c Отклонено! \u274c\u274c\u274c");
            await _client.AnswerCallbackQuery(callback.Id, "\u274c\u274c\u274c Успешно отклонено! \u274c\u274c\u274c");

        }
    }
}
