using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GetOrdersFromOpencart.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GetOrdersFromOpencart
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) 
        {

            // �������� ���� � ����� 
            //var pathToExe = Process.GetCurrentProcess().MainModule.FileName;
            // ���� � �������� �������
            //var pathToContentRoot = Path.GetDirectoryName(pathToExe);

            return Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    //logging.AddEventLog();
                    logging.AddDebug();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureServices((hostContext, services) =>
                    {
                        //services.AddHostedService<MessageSend>(); // ������� � �������, ������� ��� �����
                    });
                    webBuilder.UseStartup<Startup>();
                    //.UseSetting("detailedErrors", "true")
                    //.CaptureStartupErrors(true);
                });
        }

    }
}
