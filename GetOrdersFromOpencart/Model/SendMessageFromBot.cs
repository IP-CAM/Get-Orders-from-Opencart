using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace GetOrdersFromOpencart.Model
{
    public class SendMessageFromBot
    {
        private IConfiguration Configuration { get; }

        static ITelegramBotClient botClient;
        public SendMessageFromBot(IConfiguration configuratio)
        {
            Configuration = configuratio;
            botClient = new Telegram.Bot.TelegramBotClient(Configuration["TelegramBotClient"]);

        }

        public SendMessageFromBot(string configuratio)
        {
            botClient = new Telegram.Bot.TelegramBotClient(configuratio);

        }


        public async void Send(string textMessage)
        {
            Message message = await botClient.SendTextMessageAsync(
  //chatId: Configuration["TelegramChatId"], // or: e.Message.Chat 229187627
  chatId: 229187627,
  text: textMessage,
  parseMode: ParseMode.Markdown
);
        }


        static async void Bot_SendMessage(object sender, MessageEventArgs e)
        {
            await botClient.SendTextMessageAsync(chatId: e.Message.Chat, text: $"{e.Message.Chat.Id} + Получи фашист гранату");

        }


        static async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            if (e.Message.Text != null)
            {
                Console.WriteLine($"Получено новое сообщение в чате {e.Message.Chat.Id}.");

                await botClient.SendTextMessageAsync(
                  chatId: e.Message.Chat,
                  text: "Вы сказали:\n" + e.Message.Text
                );
            }

        }
    }
}
