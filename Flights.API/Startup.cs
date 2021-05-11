using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Flights.API
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            OpenApiInfo flightV1OpenApiInfo = new()
            {
                Version = "1.0",
                Description = "Flights API that publishes a message to RabbitMQ",
                Title = "Flights API"

            };
            services.AddSwaggerGen(swaggerGenOptions => 
            {
                swaggerGenOptions.SwaggerDoc("FlightsV1", flightV1OpenApiInfo);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseRouting();

            app.UseSwagger(swaggerOptions => { });
            app.UseSwaggerUI(swaggerUIOptions => 
            {
                swaggerUIOptions.SwaggerEndpoint("/swagger/FlightsV1/swagger.json", "Flights API");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
