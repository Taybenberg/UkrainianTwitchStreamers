using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TwitchUkrBotWorker
{
    public class Worker : IHostedService
    {
        private TwitchUkrBot.TwitchUkrBot bot;

        public Worker(BotTokens botTokens)
        {
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
