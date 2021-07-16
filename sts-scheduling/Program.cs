using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;

namespace sts_scheduling
{
    public class Program
    {
        public static void Main(string[] args)
        {
            InitializedRabbitMq();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static void InitializedRabbitMq()
        {
            var rabbitMqHostName = "localhost";
            var factory = new ConnectionFactory
            {
                HostName = rabbitMqHostName
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "request_message_queue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);
        }
    }
}
