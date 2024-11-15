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

namespace Pinner.Implementations.Commands
{
    public class SearchPhotoCommand : TextCommand
    {
        private readonly IConfiguration _config;
        public SearchPhotoCommand(string data, ITelegramBotClient client, IConfiguration config) : base(data, client)
        {
            _config = config;
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

            PythonEngine.Initialize();
            using (Py.GIL())
            {
                dynamic p = Py.Import("Scrinner");
                dynamic image = p.get_image_url(searchText);
                await _client.SendPhoto(update.Message.Chat, InputFile.FromUri((string)image), (string)image, replyMarkup: GetLikeOrDislikePhoto(), replyParameters: update.Message.MessageId);
            }

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

    }
}
