using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;

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
                            foreach (var stream in new UkrainianTwitch(TwitchApiToken).ToStringArray())
                                Bot.SendTextMessageAsync(ChatId, stream);
                            break;
                    }
                }
            };

            Bot.StartReceiving();
        }
    }
}