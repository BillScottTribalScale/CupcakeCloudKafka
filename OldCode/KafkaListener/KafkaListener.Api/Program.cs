using System.Diagnostics.CodeAnalysis;
using Manulife.Logging.DotNet;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using Steeltoe.Extensions.Configuration.CloudFoundry;

namespace KafkaListener.Api
{
    [ExcludeFromCodeCoverage]
    public class Program
    {
        public static void Main(string[] args)
        {
            var formatter = new ManulifeElkFormatter();
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.WithProperty("@version", "1.0.0")
                .Enrich.FromLogContext()
                .WriteTo.Console(formatter)
                .CreateLogger();
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)                
                .UseCloudFoundryHosting(5555)
                .AddCloudFoundry()
                .UseSerilog()
                .UseStartup<Startup>();
    }
}
