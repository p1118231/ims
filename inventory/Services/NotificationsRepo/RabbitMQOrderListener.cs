using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using inventory.Models;
using inventory.Services.NotificationRepo;
using inventory.Models.Orders; // Assuming namespace where Order model is defined


namespace inventory.Services.NotificationsRepo
;

public class RabbitMQOrderListener : BackgroundService
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public RabbitMQOrderListener(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            channel.QueueDeclare(queue: "orders",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var order = JsonSerializer.Deserialize<Order>(message);

                // Check for anomalies in the order, for example, unusually high quantity
                if (order?.OrderItems?.First().Quantity >= 100) // Assuming 100 is the threshold for an anomaly
                {
                    await _hubContext.Clients.All.SendAsync("ReceiveNotification", $"Alert: Unusually large order placed for {order?.OrderItems?.First().Quantity} units of Product {order?.OrderItems?.First().ProductId}.");
                }
                else
                {
                    await _hubContext.Clients.All.SendAsync("ReceiveNotification", $"New Order Placed: {order?.OrderItems?.First().Quantity} units of Product {order?.OrderItems?.First().ProductId}.");
                }
            };

            channel.BasicConsume(queue: "orders", autoAck: true, consumer: consumer);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
