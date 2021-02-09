using GetOrdersFromOpencart.Model;
using Microsoft.Extensions.Configuration;
using Renci.SshNet.Messages;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace GetOrdersFromOpencart.Commands
{
    public abstract class Command
    {
        private IConfiguration Configuration { get; }
        public Command(IConfiguration configuration)
        {
            Configuration = configuration;
        }
      
        public abstract string Name { get;  }
        public abstract void Execute(Telegram.Bot.Types.Message message, TelegramBotClient client);
        public bool Contains(string command)
        {
            var conf = new AppSettingsBot(Configuration);
            return command.Contains(this.Name) && command.Contains(conf.Name);
        }
    }
}
