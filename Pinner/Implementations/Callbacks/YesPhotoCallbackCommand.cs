using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pinner.Common;
using Pinner.DAL.Entities;
using Pinner.DAL.Repository.Abstractions;
using Telegram.Bot;
using Telegram.Bot.Types;
using File = System.IO.File;

namespace Pinner.Implementations.Callbacks
{
    public class YesPhotoCallbackCommand : CommandCallback
    {
        private readonly IConfiguration _config;

        public YesPhotoCallbackCommand(string data, ITelegramBotClient client, IConfiguration config) : base(data, client)
        {
            _config = config;
        }

        public override async Task ExecuteAsync(Update update, CancellationToken token)
        {
            var callback = update.CallbackQuery;
            var query = callback.Message.Text.Split("\n").ToList();
            var album = new List<InputMediaPhoto>();
            var streams = new List<FileStream>();
            var files = query.Skip(1).Select(f => Program.AppPath + "/output/" + $"{query.First()}/" + f).ToList();
            foreach (var i in files)
            {
                var stream = File.OpenRead(i);
                streams.Add(stream);
                album.Add(new InputMediaPhoto(InputFile.FromStream(stream)){Caption = "kek" });
            }
            var message =await _client.SendMediaGroup(long.Parse(_config.GetSection("ChannelPost").Value), album);
            streams.ForEach(s => s.Dispose());
            files.ForEach(f => File.Delete(f));
            await _client.EditMessageText(callback.Message.Chat, callback.Message.MessageId, "\u2705\u2705\u2705 Отправлено \u2705\u2705\u2705");
            await _client.AnswerCallbackQuery(callback.Id, "\u2705\u2705\u2705 Успешно отправлено! \u2705\u2705\u2705");
        }
    }
}
