using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

public class DeliveryService : BackgroundService
{
    private readonly ILogger<DeliveryService> _logger;

    public DeliveryService(ILogger<DeliveryService> logger)
    {
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" }; // RabbitMQ
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(queue: "shipping_requests",
                             durable: false,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var json = Encoding.UTF8.GetString(body);
            var shippingRequest = JsonSerializer.Deserialize<ShippingRequest>(json);

            // Process the shipping request and save it to CSV
            SaveToCsv(shippingRequest);
        };
        
        channel.BasicConsume(queue: "shipping_requests",
                             autoAck: true,
                             consumer: consumer);

        return Task.CompletedTask;
    }

    private void SaveToCsv(ShippingRequest shippingRequest)
    {
        var csvPath = "deliveries.csv"; // Specify your CSV path here
        using (var writer = new StreamWriter(csvPath, true))
        {
            var line = $"{shippingRequest.AfsenderAdresse},{shippingRequest.ModtagerAdresse},{shippingRequest.PakkeVægt},{DateTime.UtcNow}";
            writer.WriteLine(line);
        }

        _logger.LogInformation("Shipping request saved to CSV: {Afsender}, {Modtager}, {Vægt}", 
                                shippingRequest.AfsenderAdresse, 
                                shippingRequest.ModtagerAdresse, 
                                shippingRequest.PakkeVægt);
    }
}
