using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using app.consumer.interfaces;
using app.ApiConfiguration;
using app.DI;

namespace app
{
    public class Startup
    {
        public IConfiguration configuration { get; }

        public Startup(IConfiguration configuration) => this.configuration = configuration;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            this.configuration = configuration;
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddAutoMapper(typeof(Startup));
            services.AddApiConfig(configuration);
            services.ResolveConfigDependencies();
            services.ResolveAppDependencies();
            services.ResolveDomainDependencies();
            services.ResolveGatewayDependencies();
            services.AddControllers().AddNewtonsoftJson();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = configuration["AppInformation:name"],
                    Version = configuration["AppInformation:version"],
                    Description = configuration["AppInformation:description"]
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IRegistrationInformationConsumer consumer)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            consumer.Init();

            app.UseApiConfig(env);

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "app v1"));

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
