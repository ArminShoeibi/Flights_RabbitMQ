using Flights.Common.RabbitMQ;
using Flights.Domain;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System;
using System.Text;
using System.Text.Json;

namespace Flights.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightsController : ControllerBase
    {
        private readonly ConnectionFactory _amqpConectionFactory;
        private readonly IConnection _amqpConnection;
        private readonly IModel _amqpChannel;


        public FlightsController()
        {
            _amqpConectionFactory = new()
            {
                UserName = ConnectionFactory.DefaultUser,
                Password = ConnectionFactory.DefaultPass,
                Port = 5672,
            }; // First Step
            _amqpConnection = _amqpConectionFactory.CreateConnection(); // Second Step
            _amqpChannel = _amqpConnection.CreateModel(); // Third Step
        }

        [HttpPost("[action]")]
        public IActionResult CreateFlight(Flight flight)
        {
            /* Durability: (exchanges survive broker restart) */
            /* Auto-delete: (exchange is deleted when last queue is unbound from it) */

            _amqpChannel.ExchangeDeclare(ExchangeNames.FlightsExchange.ToString(),
                                         ExchangeType.Direct,
                                         true,
                                         true,
                                         null);


            /* Declaring a queue is idempotent - it will only be created if it doesn't exist already 
               The declaration will have no effect if the queue does already exist and
               its attributes are the same as those in the declaration.
            */

            /* Durable(the queue will survive a broker restart) */

            /* Exclusive: (An exclusive queue can only be used(consumed from, purged, deleted, etc) by its declaring connection)
               An attempt to use an exclusive queue from a different connection will result in a channel-level
               exception RESOURCE_LOCKED with an error message that says 
               cannot obtain exclusive access to locked queue.
            */

            /* Auto-delete (queue that has had at least one consumer is deleted when last consumer unsubscribes) */


            _amqpChannel.QueueDeclare(QueueNames.FlightsQueue.ToString(),
                                      true,
                                      false,
                                      false,
                                      null);

            _amqpChannel.QueueBind(QueueNames.FlightsQueue.ToString(),
                                   ExchangeNames.FlightsExchange.ToString(),
                                   RoutingKeyNames.Flights.ToString(),
                                   null);

            flight.FlightId = Guid.NewGuid();
            flight.DateCreated = DateTimeOffset.Now;
            string createFlightDtoJson = JsonSerializer.Serialize(flight);
            byte[] body = Encoding.UTF8.GetBytes(createFlightDtoJson);

            _amqpChannel.BasicPublish(ExchangeNames.FlightsExchange.ToString(),
                                      RoutingKeyNames.Flights.ToString(),
                                      null,
                                      body);

            return Ok(true);

        }
    }
}
