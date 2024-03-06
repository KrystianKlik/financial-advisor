using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace CoreMicroservice.Services
{
    public class RabbitMQService : IDisposable
    {
        private readonly ConnectionFactory _connectionFactory;
        private IConnection _connection;
        //private IModel _channel;

        public RabbitMQService()
        {
            _connectionFactory = new ConnectionFactory() { HostName = "localhost" };
            //_connection = _connectionFactory.CreateConnection();
            //_channel = _connection.CreateModel();
        }

        public void PublishMessage(string queueName, string message)
        {
            _channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);
        }

        public void ConsumeMessages(string queueName, Action<string> messageHandler)
        {
            _channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                messageHandler(message);
            };
            _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}
