using RabbitMQ.Client;
using System.Text;
using System.Threading.Channels;

ConnectionFactory factory = new();
factory.Uri = new Uri(uriString: "cloudamqp.com");
factory.ClientProvidedName = "Rabbit Sender App";

IConnection cnn = factory.CreateConnection();

IModel channel = cnn.CreateModel();

string exchangeName = "DemoExchange";
string routingKey = "demo-routing-key";
string queueName = "DemoQueue";

channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
channel.QueueDeclare(queueName, durable:false, exclusive:false, autoDelete: false, arguments:null);
channel.QueueBind(queueName, exchangeName, routingKey, arguments: null);

for (int i = 0; i < 50; i++)
{
	Console.WriteLine(value: $"Sending Message {i}");
	byte[] messageBodyByte = Encoding.UTF8.GetBytes(s: "Hello Youtube");
	channel.BasicPublish(exchangeName, routingKey, basicProperties: null, messageBodyByte);
	Thread.Sleep(millisecondsTimeout: 2000);	
}

channel.Close();
cnn.Close();
