using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

[ApiController]
[Route("api/[controller]")]
public class DeliveryController : ControllerBase
{
    private readonly ILogger<DeliveryController> _logger;
    private readonly IModel _channel;

    public DeliveryController(ILogger<DeliveryController> logger, IModel channel)
    {
        _logger = logger;
        _channel = channel;
    }

    [HttpGet]
    public IActionResult Get()
    {
        var consumer = new EventingBasicConsumer(_channel);
        var shippingRequests = new List<ShipmentDelivery>();

        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var json = Encoding.UTF8.GetString(body);
            var shipmentDelivery = JsonSerializer.Deserialize<ShipmentDelivery>(json);

            if (shipmentDelivery != null)
            {
                shippingRequests.Add(shipmentDelivery);
                _logger.LogInformation("Received shipment delivery: {MedlemsNavn}, {PickupAdresse}, {PakkeId}, {AfleveringsAdresse}",
                                        shipmentDelivery.MedlemsNavn,
                                        shipmentDelivery.PickupAdresse,
                                        shipmentDelivery.PakkeId,
                                        shipmentDelivery.AfleveringsAdresse);
            }
        };

        _channel.BasicConsume(queue: "shipping_requests", autoAck: true, consumer: consumer);

        // Return the list of shipping requests received (you might want to store these messages somewhere)
        return Ok(shippingRequests);
    }
}