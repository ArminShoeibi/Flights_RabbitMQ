using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Flights.Subscriber
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectionFactory ampqConnectionFactory = new()
            {
                Port = 5672,
                UserName = ConnectionFactory.DefaultUser,
                Password = ConnectionFactory.DefaultPass,
                HostName = "localhost",
                NetworkRecoveryInterval = TimeSpan.FromSeconds(15),
                AutomaticRecoveryEnabled = true,
            };

            IConnection amqpConnection = ampqConnectionFactory.CreateConnection();

            IModel amqpChannel = amqpConnection.CreateModel();

            EventingBasicConsumer amqpMessageConsumer = new(amqpChannel);
            amqpMessageConsumer.Received += (sender, eventArgs) =>
            {
                Console.WriteLine($"Sender: {sender}");
                Console.WriteLine($"Exchange: {eventArgs.Exchange}");
                Console.WriteLine($"Routing Key: {eventArgs.RoutingKey}");

                string flight = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
                Console.WriteLine(flight);

            };

            amqpChannel.BasicConsume("FlightsQueue", true, amqpMessageConsumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadKey();
        }
    }
}
