using BL.Interfaces;
using BL;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Model.Dto;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Interfaces;
using DAL;

namespace WordStatisticCounter
{
    public class Program
    {
        public static void Main(string[] args)
        {

            CreateHostBuilder(args).Build().Run();         

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddScoped<IRepository, Repository<wordStatisticCounterContext>>();
                    services.AddScoped<IWordCounterBO, WordCounterBO>();
                });

    }
}
