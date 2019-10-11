using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.InlineQueryResults;

namespace TwitchUkrBot
{
    public class TwitchUkrBot
    {
        readonly string TelegramApiToken;
        readonly string TwitchApiToken;

        public TwitchUkrBot(string TelegramApiToken, string TwitchApiToken)
        {
            this.TelegramApiToken = TelegramApiToken;
            this.TwitchApiToken = TwitchApiToken;

            var Bot = new TelegramBotClient(TelegramApiToken);

            Bot.SetWebhookAsync("");

            Bot.OnInlineQuery += async (object updobj, InlineQueryEventArgs iqea) =>
            {
                var list = new UkrainianTwitchStreamers.UkrainianTwitch(TwitchApiToken).ToList();
                
                var inline = new InlineQueryResultArticle[list.Count];

                for (int i = 0; i < list.Count; i++)
                {
                    var content = new InputTextMessageContent($"<b>{list[i].title}</b>\n<i>{list[i].user_name}</i> <b>|</b> <i>{list[i].name}</i>\n{list[i].url}");
                    content.ParseMode = ParseMode.Html;

                    inline[i] = new InlineQueryResultArticle(
                      i.ToString(),
                      list[i].user_name,
                      content);

                    inline[i].Description = $"{list[i].title} | {list[i].name}";

                    inline[i].ThumbUrl = list[i].url;
                }

                await Bot.AnswerInlineQueryAsync(iqea.InlineQuery.Id, inline);
            };

            Bot.OnMessage += async (object updobj, MessageEventArgs mea) =>
            {
                if (mea.Message.Type == MessageType.Text)
                {
                    var message = mea.Message;

                    if (message.Text == null)
                        return;

                    var ChatId = message.Chat.Id;

                    string command = message.Text.ToLower().Replace("@twitchukrbot", "").Replace("/", "");

                    switch (command)
                    {
                        case "start":
                            await Bot.SendTextMessageAsync(ChatId, "Вітаю! Я @TwitchUkrBot!\nНатисніть '/', щоби обрати команду.");
                            break;

                        case "streamers":
                            foreach (var stream in new UkrainianTwitchStreamers.UkrainianTwitch(TwitchApiToken).ToList())
                                Bot.SendTextMessageAsync(ChatId, $"<b>{stream.title}</b>\n<i>{stream.user_name}</i> <b>|</b> <i>{stream.name}</i>\n{stream.url}", ParseMode.Html);
                            break;

                        case "sendstreamer":
                            await Bot.SendTextMessageAsync(ChatId, "Натисніть кнопку та оберіть чат до якого хочете надіслати стрімера.", replyMarkup: new InlineKeyboardMarkup(new[] { InlineKeyboardButton.WithSwitchInlineQuery("Надіслати") }));
                            break;

                    }
                }
            };

            Bot.StartReceiving();
        }
    }
}