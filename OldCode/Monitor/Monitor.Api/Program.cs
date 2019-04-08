using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Manulife.Logging.DotNet;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Monitor
{
     public static class Program
    {
        public static void Main(string[] args)
        {
            var formatter = new ManulifeElkFormatter();
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.WithProperty("@version", "1.0.0")
                .Enrich.FromLogContext()
                .WriteTo.Console(formatter)
                .WriteTo.File(formatter, "Manulife.Logging.DotNet.Core.Log.txt", rollingInterval: RollingInterval.Day, shared: true)
                .CreateLogger();
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseSerilog()
                .UseStartup<Startup>();
    }
}
