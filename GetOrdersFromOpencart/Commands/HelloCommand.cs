using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace GetOrdersFromOpencart.Commands
{
    public class HelloCommand : Command
    {
        private IConfiguration Configuration { get; }
        public HelloCommand(IConfiguration configuration) : base(configuration)
        {
            Configuration = configuration;
        }

        public override string Name => "hello";

        public override async void Execute(Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            var messageId = message.MessageId;
            await client.SendTextMessageAsync(chatId, "Привет!", replyToMessageId: messageId);

        }
    }
}
