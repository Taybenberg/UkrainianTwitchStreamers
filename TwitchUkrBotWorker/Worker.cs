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
             *  ������� AppHarbor ������ �������������� ����� �� ����� .config, 
             *  ���� ����� �� ��������������� � .Net Core
             *  ����� �� ���������� �������� ����� �� ��������� Regex-������
             */

            var text = File.ReadAllText("TwitchUkrBotWorker.dll.config");

            var botTokens = new BotTokens()
            {
                TelegramApiToken = Regex.Match(text, "\"TelegramBotApiToken\" value=\"(.+)\"").Groups[1].Value,
                TwitchClientId = Regex.Match(text, "\"TwitchClientId\" value=\"(.+)\"").Groups[1].Value,
                TwitchSecret = Regex.Match(text, "\"TwitchSecret\" value=\"(.+)\"").Groups[1].Value
            };

            bot = new TwitchUkrBot.TwitchUkrBot(botTokens.TelegramApiToken, botTokens.TwitchClientId, botTokens.TwitchSecret);
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
