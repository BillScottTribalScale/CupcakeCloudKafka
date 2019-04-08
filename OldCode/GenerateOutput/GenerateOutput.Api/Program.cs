using System.Diagnostics.CodeAnalysis;
using Manulife.Logging.DotNet;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Serilog;

namespace GenerateOutput
{
    [ExcludeFromCodeCoverage]
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
                .CreateLogger();
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseSerilog()
                .UseStartup<Startup>();
    }
}
