using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GetOrdersFromOpencart.Model
{
    public class AppSettingsBot
    {
        private static IConfiguration Configuration { get; set; }
       public  AppSettingsBot(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public  string Url { get; set; } = Configuration["TeleSettingsBot:Url"];
        public  string Name { get; set; } = Configuration["TeleSettingsBot:Name"];
        public string Key { get; set; } = Configuration["TeleSettingsBot:Key"];

    }
}
