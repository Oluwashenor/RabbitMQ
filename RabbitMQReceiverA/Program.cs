using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Channels;

ConnectionFactory factory = new();
factory.Uri = new Uri(uriString: "amqps://eklshlub:oppXOpC8Aj49-27Q36SGGeMpuyvv8AJh@hawk.rmq.cloudamqp.com/eklshlub");
factory.ClientProvidedName = "Rabbit Receiver A App";

IConnection cnn = factory.CreateConnection();

IModel channel = cnn.CreateModel();

string exchangeName = "DemoExchange";
string routingKey = "demo-routing-key";
string queueName = "DemoQueue";

channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
channel.QueueDeclare(queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
channel.QueueBind(queueName, exchangeName, routingKey, arguments: null);
channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (sender, args) =>
{
	Task.Delay(TimeSpan.FromSeconds(value:5)).Wait();
	var body = args.Body.ToArray();
	string message = Encoding.UTF8.GetString(body);
	Console.WriteLine(value: $"Message Received:{message}");
	channel.BasicAck(args.DeliveryTag, multiple: false);
};

string consumerTag = channel.BasicConsume(queueName, autoAck: false, consumer);

Console.ReadLine();

channel.BasicCancel(consumerTag);

channel.Close();
cnn.Close(); 