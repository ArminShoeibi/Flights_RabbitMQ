using Flights.API.DTOs;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
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
        public IActionResult CreateFlight(CreateFlightDto createFlightDto)
        {
            /* Durability: (exchanges survive broker restart) */
            /* Auto-delete: (exchange is deleted when last queue is unbound from it) */

            _amqpChannel.ExchangeDeclare("FlightsExchange",
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


            _amqpChannel.QueueDeclare("FlightsQueue",
                                      true,
                                      false,
                                      false,
                                      null);

            _amqpChannel.QueueBind("FlightsQueue",
                                   "FlightsExchange",
                                   "FLRK",
                                   null);

            string createFlightDtoJson = JsonSerializer.Serialize(createFlightDto);
            byte[] body = Encoding.UTF8.GetBytes(createFlightDtoJson);

            _amqpChannel.BasicPublish("FlightsExchange",
                                      "FLRK",
                                      null,
                                      body);

            return Ok(true);

        }
    }
}
