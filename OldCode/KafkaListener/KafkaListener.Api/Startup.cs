using System;
using System.Diagnostics.CodeAnalysis;
using Common.Lib.Kafka;
using Confluent.Kafka;
using KafkaListener.Api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pivotal.Discovery.Client;
using Steeltoe.Common.Http.Discovery;
using Swashbuckle.AspNetCore.Swagger;

namespace KafkaListener.Api
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        private readonly ILogger<IKafkaConsumer> _logger;
        public Startup(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;
            _logger = loggerFactory.CreateLogger<KafkaConsumer>();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // "https://payday-calculation-engine.apps.cac.preview.pcf.manulife.com/api/calculationengine/"
            KafkaConfigSettings kafkaConfigSettings = KafkaConfigureServices.InitializeKafka(services, Configuration);
            services.AddSingleton<IHostedService, PayDayConnector>();

            services.AddDiscoveryClient(Configuration);
            services.AddSingleton<DiscoveryHttpMessageHandler>();
            
            services.AddHttpClient("payday-calc-engine", c =>
            {
                c.BaseAddress = new Uri(kafkaConfigSettings.CalculationEngineEndpoint);
            })
            .AddHttpMessageHandler<DiscoveryHttpMessageHandler>()
            .AddTypedClient<IMessageHandler<ICalculationEngineService>, CalculationEngineService>();

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Kafka Listener Manager", Version = "v1" });
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }
        
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Kafka Listener Manager V1");
                c.RoutePrefix = string.Empty;
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
            app.UseDiscoveryClient();
        }
    }
}
