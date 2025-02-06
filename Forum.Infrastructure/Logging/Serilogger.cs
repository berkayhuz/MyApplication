using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace Forum.Infrastructure.Logging
{
    public static class Serilogger
    {
        public static void ConfigureLogging()
        {
            var logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            var selfLogFile = Path.Combine(logDirectory, "serilog-selflog.txt");
            Serilog.Debugging.SelfLog.Enable(TextWriter.Synchronized(new StreamWriter(selfLogFile, true) { AutoFlush = true }));

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File(Path.Combine(logDirectory, "log-.txt"), rollingInterval: RollingInterval.Day)
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
                {
                    AutoRegisterTemplate = true,
                    IndexFormat = "logstash-{0:yyyy.MM.dd}",
                    ModifyConnectionSettings = x => x.BasicAuthentication("elastic", "Berkay16")
                })
                .CreateLogger();
        }
    }
}