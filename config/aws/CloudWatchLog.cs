using System;
using System.Linq;
using Amazon.CloudWatchLogs;
using Amazon.Runtime;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Sinks.AwsCloudWatch;

namespace config.aws
{
    public static class CloudWatchLog
    {
        public static IServiceCollection AddCloudWatchSerilog(this IServiceCollection services, string accessKey, string secretKey, string logGroupName)
        {
            var customFormatter = new CustomizedLogEventRenderer();

            var options = new CloudWatchSinkOptions()
            {
                LogGroupName = logGroupName,
                TextFormatter = customFormatter,

                MinimumLogEventLevel = LogEventLevel.Information,
                BatchSizeLimit = 100,
                QueueSizeLimit = 10000,
                Period = TimeSpan.FromSeconds(10),
                CreateLogGroup = true,
                LogStreamNameProvider = new DefaultLogStreamProvider(),
                RetryAttempts = 5
            };

            AWSCredentials credentials = new BasicAWSCredentials(accessKey, secretKey);
            IAmazonCloudWatchLogs client = new AmazonCloudWatchLogsClient(credentials, Amazon.RegionEndpoint.USEast1);

            return SetUpSerilog(services, options, client);
        }

        public static IServiceCollection AddCloudWatchSerilog(this IServiceCollection services, string logGroupName)
        {
            var customFormatter = new CustomizedLogEventRenderer();

            var options = new CloudWatchSinkOptions()
            {
                LogGroupName = logGroupName,
                TextFormatter = customFormatter,

                MinimumLogEventLevel = LogEventLevel.Information,
                BatchSizeLimit = 100,
                QueueSizeLimit = 10000,
                Period = TimeSpan.FromSeconds(10),
                CreateLogGroup = true,
                LogStreamNameProvider = new DefaultLogStreamProvider(),
                RetryAttempts = 5
            };

            IAmazonCloudWatchLogs client = new AmazonCloudWatchLogsClient();

            return SetUpSerilog(services, options, client);
        }

        public static IServiceCollection SetUpSerilog(this IServiceCollection services, CloudWatchSinkOptions options, IAmazonCloudWatchLogs client)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.WithExceptionDetails()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
                .Enrich.FromLogContext()
                .Filter.ByExcluding(c => c.Properties.Any(p => p.Value.ToString().Contains("swagger")))
                .WriteTo.AmazonCloudWatch(options, client)
                .WriteTo.Console()
                .CreateLogger();

                return services.AddSingleton(Log.Logger);
        }
    }
}