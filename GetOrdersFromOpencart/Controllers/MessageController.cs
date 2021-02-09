using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GetOrdersFromOpencart.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;
using GetOrdersFromOpencart.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace GetOrdersFromOpencart.Controllers
{

    public class TgBotController : Controller
    {
      

        CopyOrdersToNewTable copy;
        public TgBotController(CopyOrdersToNewTable cop)
        {
            copy = cop;
        }

        public IActionResult ReturnLog()
        {
            return View("StartBot");
        }

        public async Task<IActionResult> StartBot()
        {
           await copy.CopyToNewTableAsync();


            return View("StartBot", "Бот запущен, начал отслеживать добавление новых заказов в базу");
        }
    }






    [Route("api/message/update")] // вебхук маршрут
    [ApiController]
    public class MessageController : ControllerBase
    {
        private IConfiguration Configuration { get; }
        public MessageController(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        public async Task<OkResult> Update([FromBody] Update update)
        {
            var commands = Bot.Commands;
            var message = update.Message;
            var client = await new Bot(Configuration).Get();

            foreach (var command in commands)
            {
                if (command.Contains(message.Text))
                {
                    command.Execute(message, client);
                    break;
                }
            }
            return Ok();
        }
    }
}
