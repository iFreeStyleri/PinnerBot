using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pinner.Common;
using Python.Runtime;
using Telegram.Bot;
using Telegram.Bot.Types;
using Microsoft.Extensions.Configuration;
using Telegram.Bot.Types.ReplyMarkups;
using File = System.IO.File;
using Microsoft.Extensions.DependencyInjection;
using Pinner.DAL.Entities;
using Pinner.DAL.Repository.Abstractions;

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

            await _client.SendMessage(update.Message.Chat.Id, "Ищем...", replyParameters: update.Message.MessageId);
            using var topicRepository = Program.Services.GetRequiredService<IBaseRepository<Topic>>();

            var topic = await topicRepository.GetRandomRow();
            var directory = Directory.CreateDirectory(Program.AppPath + $"/output/{topic.Name}");
            var files = Directory.GetFiles(directory.FullName);
            if (files.Length == 0)
            {
                await CheckPinterest(directory.FullName, topic);
                files = Directory.GetFiles(Program.AppPath + $"/output/{topic.Name}");
                await SendMedia(update, files, topic);
            }
            else
                await SendMedia(update, files, topic);

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

        private async Task CheckPinterest(string directory, Topic topic)
        {
            using var tagRepository = Program.Services.GetRequiredService<IBaseRepository<Tag>>();
            var tag = await tagRepository.GetAll()
                .Include(f => f.Topic)
                .Where(w => w.Topic.Id == topic.Id)
                .GroupBy(g => EF.Functions.Random()).Select(s => s.First())
                .FirstOrDefaultAsync();

            await Task.Run(() =>
            {
                PythonEngine.Initialize();
                using (Py.GIL())
                {
                    dynamic p = Py.Import("Scrinner");
                    p.get_image_url(tag?.Query, directory, 100);
                }
            });
        }

        private async Task SendMedia(Update update, string[] files,Topic topic)
        {
            if (files.Length != 0)
            {
                var query = new List<string> {topic.Name};
                List<FileStream> streams = new List<FileStream>();
                var album = new List<InputMediaPhoto>();
                if (files.Length <= 3)
                {
                    query.AddRange(files.Select(s => Path.GetFileName(s))); 
                    foreach (var file in query.Skip(1))
                    {
                        var stream = File.Open(file, FileMode.Open);
                        streams.Add(stream);
                        album.Add(new InputMediaPhoto(InputFile.FromStream(stream)));
                    }
                    var message = await _client.SendMediaGroup(update.Message.Chat, album, replyParameters: update.Message.MessageId);
                    await _client.SendMessage(update.Message.Chat, string.Join("\n", query), replyMarkup: GetLikeOrDislikePhoto(), replyParameters: message.First().MessageId);
                    streams.ForEach(f => f.Dispose());
                }
                else
                {
                    var count = _rand.Next(1, 3);
                    for (int i = 0; i < count; i++)
                    {
                        var selectIndex = _rand.Next(0, files.Length - 1);
                        query.Add(Path.GetFileName(files[selectIndex]));
                        var stream = File.Open(files[selectIndex], FileMode.Open);
                        streams.Add(stream);
                        album.Add(new InputMediaPhoto(InputFile.FromStream(stream)));
                    }
                    var message = await _client.SendMediaGroup(update.Message.Chat, album, replyParameters: update.Message.MessageId);
                    await _client.SendMessage(update.Message.Chat, string.Join("\n", query), replyMarkup: GetLikeOrDislikePhoto(), replyParameters: message.First().MessageId);
                    streams.ForEach(f =>
                    {
                        f.Close();
                        f.Dispose();
                    });

                }
            }

        }
    }
}
