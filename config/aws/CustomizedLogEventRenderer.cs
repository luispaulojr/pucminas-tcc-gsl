using System.IO;
using System.Text;
using Serilog.Events;
using Serilog.Formatting;
namespace config.aws
{
    public class CustomizedLogEventRenderer : ITextFormatter
    {
        public void Format(LogEvent logEvent, TextWriter output) => output.Write(format: this.RenderLogEvent(logEvent));

        private string RenderLogEvent(LogEvent logEvent)
        {
            var builder = new StringBuilder();

            builder.AppendLine(value: $"Timestamp: [{logEvent.Timestamp}]");
            builder.AppendLine(value: $"Level: [{logEvent.Level}]");
            builder.AppendLine(value: $"Message: [{logEvent.RenderMessage()}]");

            if(logEvent.Exception == null) return builder.ToString();

            builder.AppendLine(value: $"Exception: [{logEvent.Exception}]");

            if(logEvent.Properties != null)
            {
                var details = new StringBuilder();

                foreach (var propertie in logEvent.Properties)
                {
                    details.Append(value: $"{propertie.Key}: {propertie.Value} ");
                }

                builder.AppendLine($"Exception details: {details}");
            }
            builder.AppendLine($"ExceptionDetails: [{logEvent.Exception}]");

            return builder.ToString();
        }
    }
}