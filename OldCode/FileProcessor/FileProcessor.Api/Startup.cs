using FileProcessor.Api.Services;
using FileProcessor.Api.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Confluent.Kafka;
using System.Diagnostics.CodeAnalysis;
using Common.Lib.Kafka;

namespace FileProcessor
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
            KafkaConfigSettings kafkaConfigSettings = KafkaConfigureServices.InitializeKafka(services, Configuration);           
            services.AddSingleton<IFileProcessorService, FileProcessorService>();
            services.AddSingleton<IFileManager, FileManager>();
            services.AddSingleton<IMessageManager, MessageManager>();
            services.AddSingleton<IMessagePublisher, MessagePublisher>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new Info { Title = "File Processor", Version = "v1" });
                });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "File Processor V1");
            });
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
