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
    public class YesPhotoCallbackCommand : CommandCallback
    {
        public YesPhotoCallbackCommand(string data, ITelegramBotClient client) : base(data, client)
        {

        }

        public override async Task ExecuteAsync(Update update, CancellationToken token)
        {
            var callback = update.CallbackQuery;
            var imgs = callback.Message.Text.Split("\n").ToList();
            var album = new List<InputMediaPhoto>();
            var streams = new List<FileStream>();
            var files = imgs.Select(f => Program.AppPath + "/output/" + f).ToList();
            foreach (var i in files)
            {
                var stream = File.OpenRead(i);
                streams.Add(stream);
                album.Add(new InputMediaPhoto(InputFile.FromStream(stream)));
            }
            var message =await _client.SendMediaGroup(-1002312040693, album);
            await _client.EditMessageCaption(-1002312040693, message.First().MessageId, "kek");
            streams.ForEach(s => s.Dispose());
            files.ForEach(f => File.Delete(f));
            await _client.AnswerCallbackQuery(callback.Id, "Успешно отправлено!");
        }
    }
}
