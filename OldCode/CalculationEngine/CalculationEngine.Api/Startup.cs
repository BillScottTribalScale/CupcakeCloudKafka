using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Confluent.Kafka;
using CalculationEngine.Api.Services;
using CalculationEngine.Api.Models;
using System.Diagnostics.CodeAnalysis;
using Pivotal.Discovery.Client;
using Common.Lib.Kafka;

namespace CalculationEngine
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            KafkaConfigSettings kafkaConfigSettings = KafkaConfigureServices.InitializeKafka(services, Configuration);
            services.AddSingleton<ICalculationEngineService, CalculationEngineService>();
            services.AddSingleton<IMessagePublisher, MessagePublisher>();
            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new Info { Title = "Calculation Engine", Version = "v1" });
                });

            services.AddDiscoveryClient(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Calculation Engine V1");
            });

            app.UseHttpsRedirection();
            app.UseMvc();
            app.UseDiscoveryClient();
        }
    }
}
