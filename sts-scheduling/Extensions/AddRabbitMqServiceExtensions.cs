using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using sts_scheduling.Models.Requests;
using sts_scheduling.Service.Implementations;
using sts_scheduling.Utils;
using System.Text;

namespace sts_scheduling.Extensions
{
    public static class AddRabbitMqServiceExtensions
    {
        public static IServiceCollection AddRabbitMQService(
            this IServiceCollection services, IConfiguration configuration)
        {
            var factory = new ConnectionFactory
            {
                VirtualHost = configuration
                    .GetSection("RabbitMqHeroku")["VirtualHost"],
                HostName = configuration
                    .GetSection("RabbitMqHeroku")["HostName"],
                UserName = configuration
                    .GetSection("RabbitMqHeroku")["UserName"],
                Password = configuration
                    .GetSection("RabbitMqHeroku")["Password"]
            };

            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "sts_api_request",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var scheduleRequest = JsonConvert
                    .DeserializeObject<ScheduleRequest>(message);

                ScheduleService service = new();
                var scheduleResponse = service.ComputeSchedule(scheduleRequest);
                scheduleResponse.ShiftScheduleResultId = scheduleRequest.Id;
                scheduleResponse.StoreId = scheduleRequest.StoreId;

                SendHttpRequest.SendScheduleResult(scheduleResponse).Wait();

                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };
            channel.BasicConsume(queue: "sts_api_request",
                autoAck: false,
                consumer: consumer);

            services.AddSingleton(channel);

            return services;
        }
    }
}
