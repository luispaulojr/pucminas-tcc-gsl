using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using app.consumer.interfaces;
using app.ApiConfiguration;
using config.aws;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Amazon.Extensions.NETCore.Setup;

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
            // services.AddDefaultAWSOptions(GetAWSOptions());
            services.AddAutoMapper(typeof(Startup));
            services.AddApiConfig(configuration);
            services.ResolveConfigDependencies();
            services.ResolveAppDependencies();
            services.ResolveDomainDependencies();
            services.ResolveGatewayDependencies();
            services.AddControllers().AddNewtonsoftJson();
            services.AddControllers();
            // services.AddCloudWatchSerilog(
            //     #if DEBUG
            //         configuration.GetValue<string>("AWS:AccessKey"), configuration.GetValue<string>("AWS:SecretKey"), configuration.GetValue<string>("AWS:LogGroup")
            //     #else
            //         configuration.GetValue<string>("AWS:LogGroup")
            //     #endif
            // );
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IRegistrationInformationConsumer consumer, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            consumer.Init();

            app.UseApiConfig(env);

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "app v1"));

            app.UseSwaggerUI(options =>
            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                }
            });

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }

        private AWSOptions GetAWSOptions() => this.configuration.GetAWSOptions();
    }
}
