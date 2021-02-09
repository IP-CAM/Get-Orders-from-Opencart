using GetOrdersFromOpencart.Model;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace GetOrdersFromOpencart.Infrastructure
{
    public class SendBlazorService : BackgroundService
    {
        CopyOrdersToNewTable meth;

        public SendBlazorService(CopyOrdersToNewTable cc)
        {
            meth = cc;
        }

        private static System.Timers.Timer timer = new System.Timers.Timer();
        public void StartTimer()
        {
            UpdateTimer();
        }
        public async Task StartTimer2()
        {
            await UpdateTimer();
        }

        public async Task UpdateTimer()
        {

            timer.AutoReset = true;
            timer.Interval = 3 * 60 * 1000;
            timer.Elapsed += onTimeEvent;
            timer.Enabled = true;
        }

        public async void onTimeEvent(Object source, ElapsedEventArgs e)
        {
            await meth.CopyToNewTableAsync();
            Updatedate(DateTime.Now);
        }



        public event Func<DateTime, Task> vremyaProverki;

        public async void Updatedate(DateTime dateTime)
        {
           await vremyaProverki.Invoke(dateTime);
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await StartTimer2();
            }
        }


    }
}
