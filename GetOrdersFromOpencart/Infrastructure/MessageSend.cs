using GetOrdersFromOpencart.Model;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GetOrdersFromOpencart.Infrastructure
{
    public class MessageSend : BackgroundService
    {
        private readonly ILogger<MessageSend> _logger;
        CopyOrdersToNewTable meth;

        public MessageSend(CopyOrdersToNewTable cc, ILogger<MessageSend> logger)
        {
            meth = cc;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await meth.CopyToNewTableAsync();
                _logger.LogInformation("Запрос на проверку новых заказов запущен в: {time}", DateTimeOffset.Now);
                await Task.Delay(3 * 60 * 1000, stoppingToken);
            }
        }
    }
}
