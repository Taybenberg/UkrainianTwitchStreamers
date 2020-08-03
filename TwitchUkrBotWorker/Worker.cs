using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace TwitchUkrBotWorker
{
    public class Worker : IHostedService
    {
        private TwitchUkrBot.TwitchUkrBot bot;

        public Worker()
        {
            /*  
             *  Хостинг AppHarbor записує конфігураційні змінні до файлу .config, 
             *  який більше не використовується в .Net Core
             *  Через це доводиться діставати токен за допомогою Regex-виразу
             */

            var text = File.ReadAllText("TwitchUkrBotWorker.dll.config");

            var botTokens = new BotTokens()
            {
                TelegramApiToken = Regex.Match(text, "\"TelegramBotApiToken\" value=\"(.+)\"").Groups[1].Value,
                TwitchApiToken = Regex.Match(text, "\"TwitchApiToken\" value=\"(.+)\"").Groups[1].Value
            };

            bot = new TwitchUkrBot.TwitchUkrBot(botTokens.TelegramApiToken, botTokens.TwitchApiToken);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            bot.Start();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            bot.Stop();

            return Task.CompletedTask;
        }
    }
}
