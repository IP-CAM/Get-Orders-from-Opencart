using GetOrdersFromOpencart.Commands;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;

namespace GetOrdersFromOpencart.Model
{
    public class Bot
    {
        private IConfiguration Configu { get; }
        public Bot(IConfiguration configuration)
        {
            Configu = configuration;
        }

        private static TelegramBotClient client;
        private static List<Command> comandsList;
        public static IReadOnlyList<Command> Commands { get => comandsList.AsReadOnly(); }
        public async Task<TelegramBotClient> Get()
        {
            if (client != null)
            {
                return client;
            }
            comandsList = new List<Command>();
            comandsList.Add(new HelloCommand(Configu));
            // тут добавляю другие комманды\

            var conf = new AppSettingsBot(Configu);
            client = new TelegramBotClient(conf.Key);
            var hook = string.Format(conf.Url, "api/message/update");
            await client.SetWebhookAsync(hook);
            return client;
        }

    }
}
