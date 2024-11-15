using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pinner.Common;
using Python.Runtime;
using Telegram.Bot;
using Telegram.Bot.Types;
using Microsoft.Extensions.Configuration;
using Telegram.Bot.Types.ReplyMarkups;
using File = System.IO.File;

namespace Pinner.Implementations.Commands
{
    public class SearchPhotoCommand : TextCommand
    {
        private readonly Random _rand;
        private readonly IConfiguration _config;
        public SearchPhotoCommand(string data, ITelegramBotClient client, IConfiguration config) : base(data, client)
        {
            _config = config;
            _rand = new Random();
        }

        public override async Task ExecuteAsync(Update update, CancellationToken token)
        {
            var admins = _config.GetSection("Admins").Get<long[]>();
            if (!admins.Any(s => s == update.Message.From.Id))
            {
                await _client.SendMessage(update.Message.Chat.Id, "Вне доступа");
                return;
            }
            var searchText = update.Message.Text.Replace("/search ", "").Replace("/search@SkyEllegentBot ", "");
            if (string.IsNullOrWhiteSpace(searchText)) return;

            if (searchText.Length <= 3)
            {
                await _client.SendMessage(update.Message.Chat.Id, $"Искомая картинка по тексу '{searchText}' меньше 3-х символов", replyParameters: update.Message.MessageId);
                return;
            }
            await _client.SendMessage(update.Message.Chat.Id, "Ищем...", replyParameters: update.Message.MessageId);

            var files = Directory.GetFiles(Program.AppPath + "/output");
            if (files.Length == 0)
            { 
                await CheckPinterest(searchText);
                files = Directory.GetFiles(Program.AppPath + "/output");
                await SendMedia(update, files);
            }
            else
                await SendMedia(update, files);
        }
        private IReplyMarkup GetLikeOrDislikePhoto()
            => new InlineKeyboardMarkup(new List<List<InlineKeyboardButton>>()
            {
                new List<InlineKeyboardButton>()
                {
                    new InlineKeyboardButton("\u2705 Да \u2705") {CallbackData = "Yes Photo"},
                    new InlineKeyboardButton("\u26d4\ufe0f No \u26d4\ufe0f") {CallbackData = "No Photo"}
                }
            });

        private async Task CheckPinterest(string searchText)
        {
            await Task.Run(() =>
            {
                PythonEngine.Initialize();
                using (Py.GIL())
                {
                    dynamic p = Py.Import("Scrinner");
                    p.get_image_url(searchText);
                }
            });
        }

        private async Task SendMedia(Update update, string[] files)
        {
            if (files.Length != 0)
            {
                List<FileStream> streams = new List<FileStream>();
                var album = new List<InputMediaPhoto>();
                if (files.Length <= 3)
                {
                    var fileNames = files.Select(s => Path.GetFileName(s));
                    foreach (var file in fileNames)
                    {
                        var stream = File.Open(file, FileMode.Open);
                        streams.Add(stream);
                        album.Add(new InputMediaPhoto(InputFile.FromStream(stream)));
                    }
                    var message = await _client.SendMediaGroup(update.Message.Chat, album, replyParameters: update.Message.MessageId);
                    await _client.SendMessage(update.Message.Chat, string.Join("\n", fileNames), replyMarkup: GetLikeOrDislikePhoto(), replyParameters: update.Message.MessageId);
                    streams.ForEach(f => f.Dispose());
                }
                else
                {
                    var fileNames = new List<string>();
                    var count = _rand.Next(1, 3);
                    for (int i = 0; i < count; i++)
                    {
                        var selectIndex = _rand.Next(0, files.Length - 1);
                        fileNames.Add(Path.GetFileName(files[selectIndex]));
                        var stream = File.Open(files[selectIndex], FileMode.Open);
                        streams.Add(stream);
                        album.Add(new InputMediaPhoto(InputFile.FromStream(stream)));
                    }
                    var message = await _client.SendMediaGroup(update.Message.Chat, album, replyParameters: update.Message.MessageId);
                    await _client.SendMessage(update.Message.Chat, string.Join("\n", fileNames), replyMarkup: GetLikeOrDislikePhoto(), replyParameters: update.Message.MessageId);
                    streams.ForEach(f => f.Dispose());

                }
            }

        }
    }
}
